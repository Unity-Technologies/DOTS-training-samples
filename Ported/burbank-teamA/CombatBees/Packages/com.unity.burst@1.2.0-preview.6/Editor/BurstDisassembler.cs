using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Unity.Burst.Editor
{
    /// <summary>
    /// Improves the output of the default burst disassembler by adding colors
    /// </summary>
    internal class BurstDisassembler
    {
        private readonly Dictionary<int, string> _fileName;
        private readonly Dictionary<int, string[]> _fileList;

        public BurstDisassembler()
        {
            _fileName= new Dictionary<int, string>();
            _fileList=new Dictionary<int, string[]>();
        }

        public string Process(string input)
        {
            _fileList.Clear();
            _fileName.Clear();
            var tokenizer = new GASTokenizer();

            var tokens = tokenizer.Tokenize(input).ToList();

            var output = new StringBuilder();
            int prevStart = 0;
            for (int tokenIdx = 0; tokenIdx < tokens.Count; tokenIdx++)
            {
                var t = tokens[tokenIdx];
                if (t.Token == TokenType.Preprocessor)
                {
                    if (t.Literal == ".file")
                    {
                        int start = t.Start;
                        // File is followed by an index and a string or just a string with an implied index = 0
                        tokenIdx++;
                        int index = 0;
                        if (tokens[tokenIdx].Token == TokenType.Number)
                        {
                            index = int.Parse(tokens[tokenIdx].Literal);
                            tokenIdx++;
                        }

                        if (tokens[tokenIdx].Token == TokenType.String)
                        {
                            var filename = tokens[tokenIdx].Literal.Trim('"');
                            string[] fileLines;

                            try
                            {
                                fileLines = System.IO.File.ReadAllLines(filename);
                            }
                            catch
                            {
                                fileLines = null;
                            }


                            _fileName.Add(index, filename);
                            _fileList.Add(index, fileLines);
                        }

                        while (tokens[tokenIdx].Token != TokenType.EndOfLine)
                        {
                            tokenIdx++;
                            if (tokens[tokenIdx].Token == TokenType.EndOfInput)
                                break;
                        }

                        output.Append(input, prevStart, start - prevStart);
                        prevStart = tokens[tokenIdx].Start;
                        continue;
                    }

                    if (t.Literal == ".loc")
                    {
                        int start = t.Start;
                        // Fileno lineno [column] [options] -
                        int fileno = 0;
                        int lineno = 0;        // NB 0 indicates no information given
                        tokenIdx++;
                        if (tokens[tokenIdx].Token == TokenType.Number)
                        {
                            fileno = int.Parse(tokens[tokenIdx].Literal);
                            tokenIdx++;
                        }
                        if (tokens[tokenIdx].Token == TokenType.Number)
                        {
                            lineno = int.Parse(tokens[tokenIdx].Literal);
                            tokenIdx++;
                        }

                        while (tokens[tokenIdx].Token != TokenType.EndOfLine)
                        {
                            tokenIdx++;
                            if (tokens[tokenIdx].Token == TokenType.EndOfInput)
                                break;
                        }

                        output.Append(input, prevStart, start - prevStart);
                        prevStart = tokens[tokenIdx].Start;

                        // If the file is 0, then its unknown, we should just reset any file tracking information
                        if (fileno == 0)
                        {
                            uint color = 0xFFFF00FF;
                            output.Append("<color=#"+color.ToString("x8")+$">(-)</color>");
                        }
                        // If the line number is 0, then we can update the file tracking, but still not output a line
                        else if (lineno == 0)
                        {
                            uint color = 0xFFFF00FF;
                            output.Append("<color=#" +color.ToString("x8")+$">({fileno} : {System.IO.Path.GetFileName(_fileName[fileno])})</color>");

                        }
                        // We have a source line and number -- can we load file and extract this line?
                        else
                        {
                            lineno--;
                            if (_fileList.ContainsKey(fileno) && _fileList[fileno] != null && lineno<_fileList[fileno].Length)
                            {
                                uint color = 0xFFFF00FF; //(0x11110000u * (uint)lineno)+0xFFu;
                                output.Append("<color=#" + color.ToString("x8") + $">({fileno},{lineno} : {System.IO.Path.GetFileName(_fileName[fileno])})" + _fileList[fileno][lineno] + "</color>\n");
                            }
                            else
                            {
                                uint color = 0xFFFF00FF;//(0x11110000u * (uint)lineno)+0xFFu;
                                output.Append("<color=#"+color.ToString("x8")+$">({fileno},{lineno} : {System.IO.Path.GetFileName(_fileName[fileno])})</color>");
                            }
                        }
                        continue;

                    }
                }

                if (t.Token == TokenType.EndOfInput)
                {
                    output.Append(input.Substring(prevStart));
                    break;
                }

            }

            return output.ToString();
        }

        private enum TokenType
        {
            Preprocessor,
            String,
            Number,
            HexNumber,
            Label,
            Comma,
            Other,
            EndOfLine,
            EndOfInput
        }

        private struct TokenMatch
        {
            public TokenType Token { get; set; }
            public string Literal { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public int Priority { get; set; }
        }

        private class GASToken
        {
            private readonly Regex _matchExpression;
            private readonly TokenType _tokenType;
            private readonly int _matchPriority;

            public GASToken(string matchPattern, TokenType type, int priority)
            {
                _matchExpression = new Regex(matchPattern, RegexOptions.Compiled | RegexOptions.Multiline);
                _tokenType = type;
                _matchPriority = priority;

            }

            public IEnumerable<TokenMatch> FindMatches(string toSearch)
            {
                MatchCollection matches = _matchExpression.Matches(toSearch);
                foreach (Match match in matches)
                {
                    yield return new TokenMatch()
                    {
                        Token = _tokenType,
                        Literal = match.Groups.Count > 1 ? match.Groups[1].Value : match.Value,
                        StartIndex = match.Index,
                        EndIndex = match.Index + match.Length,
                        Priority = _matchPriority
                    };
                }
            }
        }

        private struct TokenValue
        {
            public TokenValue(TokenType type)
            {
                Token = type;
                Literal = "";
                Start = 1;
                End = 0;
            }

            public TokenValue(TokenType type, string value, int start, int end)
            {
                Token = type;
                Literal = value;
                Start = start;
                End = end;
            }

            public TokenType Token { get; }
            public string Literal { get; }
            public int Start { get; }
            public int End { get; }

            public override string ToString()
            {
                return $"{nameof(Token)}: {Token}, {nameof(Literal)}: {Literal}, {nameof(Start)}: {Start}, {nameof(End)}: {End}";
            }
        }

        // TODO: The Tokenizer is not optimized speed/memory wise (using regex)
        private class GASTokenizer
        {
            private readonly List<GASToken> _tokens;

            public GASTokenizer()
            {
                _tokens = new List<GASToken>
                {
                    new GASToken(@"^\s*(\.[\w._]+)", TokenType.Preprocessor, 2),
                    new GASToken(@"^\.?[\w._]+:?", TokenType.Label, 1),
                    new GASToken(@"""[^""]*""", TokenType.String, 1),
                    new GASToken(@"\d+", TokenType.Number, 2),
                    new GASToken(@"0x[0-9a-fA-F]+", TokenType.HexNumber, 1),
                    new GASToken(@",", TokenType.Comma, 1),
                    new GASToken(@"$", TokenType.EndOfLine, 1),
                    new GASToken(@"[\w._]+", TokenType.Other, 3)
                };
            }

            public IEnumerable<TokenValue> Tokenize(string input)
            {
                var tokenMatches = TokenizeMatches(input);

                var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                    .OrderBy(x => x.Key)
                    .ToList();

                var lastMatch = new TokenMatch();
                for (int i = 0; i < groupedByIndex.Count; i++)
                {
                    var bestMatch = groupedByIndex[i].OrderBy(x => x.Priority).First();
                    if (lastMatch.Literal != null && bestMatch.StartIndex < lastMatch.EndIndex)
                        continue;

                    yield return new TokenValue(bestMatch.Token, bestMatch.Literal, bestMatch.StartIndex, bestMatch.EndIndex);

                    lastMatch = bestMatch;
                }

                yield return new TokenValue(TokenType.EndOfInput);
            }

            private List<TokenMatch> TokenizeMatches(string input)
            {
                var matches = new List<TokenMatch>();
                foreach (var token in _tokens)
                {
                    matches.AddRange(token.FindMatches(input));
                }

                return matches;
            }
        }
    }
}

