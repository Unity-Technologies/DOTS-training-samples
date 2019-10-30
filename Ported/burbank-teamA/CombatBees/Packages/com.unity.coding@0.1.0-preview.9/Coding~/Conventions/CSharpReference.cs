// | CODING STANDARD-COMPLIANT REFERENCE C# FILE
// |   _____     ____
// |  / ___/  __/ / /_
// | / /__   /_  . __/
// | \___/  /_  . __/
// |         /_/_/
// |
// |[About this file]
// |    - This is a 'living document' and will be updated as our standard evolves.
// |    - For best rendering, be sure to view this file in your text editor rather than through a web site source viewer.
// |    - The '// |'-style comments denote documentation markup, and are not part of the actual sample.
// |    - To avoid redundancy, rules inline in the reference code are only mentioned once. Assume they apply generally unless noted, or if obvious from the context.
// |    - This code is only intended to demonstrate conventions, and as a result sometimes gets nonsensical. Pay no attention to the substance, only the form.
// |    - Reasoning behind rules are not included here to save space. Ask on Q or browse 'code-conventions + c#' at https://q.unity3d.com/search.html?f=&type=question&redirect=search%2Fsearch&sort=relevance&q=%5Bcode-conventions%5D+and+%5Bc%23%5D.
// |    - This code will always compile cleanly as a Unity script and will show as fully green in ReSharper using the Unity rule set.
// |    - This file is currently maintained by scobi and lives in https://ono.unity3d.com/unity-extra/unity-meta/raw/@/ReferenceSource/CSharp/Assets/CSharpReference.cs.
// |    - For clarification, or reporting of ambiguities or bugs, ask on Q or #devs-code-conventions in Slack.
// |    - If you are writing C# intended for the Unity runtime (like UnityEngine.dll) then also read up on https://q.unity3d.com/questions/1814/what-should-i-consider-when-writing-c-code-that-wi.html
// |
// |[General]
// |    - Our standard is derived in part from Microsoft's public standards (see http://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx).
// |    - If there is any disagreement between this file and Microsoft's standards, this file always wins.
// |    - If the compiler does not require something, leave it out (i.e. 'this.' prefix, default access levels, 'Attribute' postfix, etc.)
// |
// |[Encoding]
// |    - Text file encoding is UTF8 with no BOM, using LF (unix) line endings.
// |    - 4-wide tabstops, using spaces only (no tab characters)
// |    - No trailing whitespace on lines, but always include a single newline at the end of the file.
// |    - (All of the above are ensured by a combination of automated tools. Make sure you have followed the setup instructions at http://confluence.hq.unity3d.com/x/ooPD.)
// |
// |[Files]
// |    - No file header, copyright, etc. of any kind. Some IDE's may add them automatically - please remove them.
// |    - Maintain the style of surrounding code if it has its own separate standard (i.e. is or heavily derived from external).
// |    - Use PascalCase for file names, typically matching the name of the dominant type in the file (or if none is dominant, use a reasonable category name).
// |
// |[Naming]
// |    - Use PascalCase for all symbol names, except where noted.
// |    - No 'Hungarian notation' or other prefixes, except where noted.
// |    - Spell words using correct US-English spelling. Note that there are a few legacy exceptions that use GB-English that we must preserve, but do not add new ones.
// |    - Use descriptive and accurate names, even if it makes them longer. Favor readability over brevity.
// |    - Avoid abbreviations when possible unless the abbreviation is commonly accepted.
// |    - Acronyms are UPPERCASE, unless they are the first word in a camelCase word. For example, CPUTimer vs. tpsReport.
// |    - Do not capitalize each word in so-called closed-form compound words (see http://msdn.microsoft.com/en-us/library/ms229043(v=vs.110).aspx for a sample list of compound words)
// |    - Use semantically interesting names rather than language-specific keywords for type names (i.e. GetLength > GetInt).
// |    - Use a common name, such as value/item/element, rather than repeating the type name, in the rare cases when an identifier has no semantic meaning and the type is not important (i.e. newElements > newInts).
// |
// |    Definitions:
// |        - camelCase: words* capitalized, except the first (see the humps?)
// |        - PascalCase: all words* capitalized
// |        - UPPERCASE: all letters in all words* capitalized
// |        * A "word" may only contain letters and numbers (no underscores or other symbols).
// |
// |    Readability examples:
// |        - HorizontalAlignment instead of AlignmentHorizontal (more English-readable)
// |        - CanScrollHorizontally instead of ScrollableX ('x' is somewhat obscure reference to the x axis)
// |        - DirectionalVector instead of DirVec (unnecessary and use of nonstandard abbreviation)
// |
// |    Common abbreviations:
// |        - param (parameter), arg (argument), id (identifier), db (database), ok (okay)
// |
// |[Spacing]
// |    - Space before opening parenthesis?
// |        - If it looks like a function call, no space (function calls, function definitions, typeof(), sizeof())
// |        - If it opens a scope, add a space (if, while, catch, switch, for, foreach, using, lock, fixed)
// |    - No spaces immediately inside any parens or brackets (e.g. no 'if ( foo )' or 'x = ( y * z[ 123 ] )')
// |    - Comma and semicolon spacing as in English ('int a, float b' and 'for (int i = 0; i < 10; ++i)')
// |    - Exactly one space is required after the // in a C++ style comment.
// |    - Do not add a space between a unary operator and its operand (!expr, +30, -1.4, i++, --j, &expr, *expr, (int)obj, etc.).
// |    - Do not add spaces around member access operators (a.b, a->b, etc.).
// |    - Spaces are required both before and after all other operators (math, assignment, comparison, lambdas, etc.).
// |
// |[Wrapping]
// |    - Wrap code once it gets to around 120 columns wide to keep side-by-side diffs sane (not a hard limit; use your judgment).
// |    - When necessary, break lines after boolean operators in conditional expressions, after ';' in for-statements, and after ',' in function calls
// |
// |[Comments]
// |    - Documenting the 'why' is far more important than the 'what' or 'how'.
// |    - Document anything that would surprise another engineer (or yourself in six months when you've forgotten it).
// |    - /*C-Style comments*/ are not permitted. They are reserved for commenting out big hunks of code locally (never to be committed).
// |    - No "divider" comments (i.e. long ----- and ////// comments just to break up code).
// |    - No "category" comments (i.e. // Functions  // Private Data  // Globals etc.).
// |    - Use of #region is _always_ disallowed.
// |    - Only use /// (triple slash) comments if you are writing xmldoc, and never for ordinary comments that you want to stand out
// |________________________________________________________________________________________________

// |[Usings]
// |    - Located at file scope at the top of the file, never within a namespace.
// |    - Three groups, which are, top to bottom: System, non-System, aliases. Keep each group sorted.
// |    - Strip unused 'usings' except the 'minimally-required set', which is marked with *required below.
// |    - Only use aliases when required by the compiler for disambiguation, and not for hiding rarely-used symbols behind a prefix.
// |    - Always drop explicit namespace qualifications on types when a 'using' can be added (i.e. almost all of the time).
using System;                                                                                       // | Not required, but strongly encouraged
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Company.BuildSystem;                                                                          // | Start of non-System group
using Microsoft.Win32;
using UnityEngine;

using Component = UnityEngine.Component;                                                            // | Start of aliases group
using Debug = UnityEngine.Debug;

namespace UnityEditor                                                                               // | Full contents of namespace indented
{
    // |[Enums]
    // |    - Use a singular type name, and no prefix or suffix (e.g. no E- prefix or -Enum suffix).
    // |    - Constant names should have no prefix or suffix.
    // |    - Do not specify constant values unless absolutely required (e.g. for version-safe protocols - rare).
    enum WindingOrder                                                                               // | Drop redundant access specifiers (leave off 'internal' at file scope)
    {                                                                                               // | Opening brace is always on its own line at the same level of indentation as its parent
        Clockwise,                                                                                  // | Code within the braces always indented one tab stop
        CounterClockwise,
        Charm,
        Singularity,                                                                                // | Trail last element in a list with ','
    }                                                                                               // | Closing brace is on its own line at same level of indentation as parent
                                                                                                    // | Put exactly one blank line between multi-line types
    // |[Flags enums]
    // |    - Use a plural type name, and no prefix or suffix (e.g. no E- prefix and no -Flag or -Flags suffix).
    // |    - Constant names should have no prefix or suffix.
    // |    - Use column-aligned bit shift expressions for the constants (instead of 2, 4, 8, etc.)
    [Flags]
    public enum VertexStreams
    {
        Position    = 1 << 0,
        Normal      = 1 << 1,
        Tangent     = 1 << 2,
        Color       = 1 << 3,
        UV          = 1 << 4,
    }

    // |[Interfaces]
    // |    - Name interfaces with adjective phrases, or occasionally with nouns or noun phrases.
    // |        - Nouns and noun phrases should be used rarely and they might indicate that the type should be an abstract class, and not an interface.
    // |    - Use 'I' prefix to indicate an interface.
    // |    - Ensure that the names differ only by the 'I' prefix on the interface name when you are defining a class-interface pair, where the class is a standard implementation of the interface.
    public interface IThingAgent
    {
        string operationDescription { get; }
        float scale { get; }

        // |[Methods]
        // |    - Give methods names that are verbs or verb phrases.
        // |    - Parameter names are camelCase
        bool DoThing(string propertyDescription, int spinCount);
    }

    // |[Classes]
    // |    - Name classes and structs with nouns or noun phrases.
    // |    - No prefix on class names (no 'C' or 'S' etc.).
    class Example
    {
        // |[Fields]
        // |    - Use prefix + PascalCase for non-public field naming.
        // |        - Prefixes: m_ = instance field, s_ = static readwrite field, k_ = const
        // |        - Also prefix static/instance readonly with k_ if the intent is to treat the field as deeply const.
        // |    - Drop redundant initializers (i.e. no '= 0' on the ints, '= null' on ref types, etc.).
        // |    - Drop redundant access specifiers (leave off 'private' at type scope).
        // |    - Never expose fields in classes as public, always publish through a property.
        // |    - Use readonly where const isn't possible.
        static readonly Vector3 k_DefaultLength = new Vector3(1, 2, 3);                             // | When it enhances readability, try to column-align blocks of variable definitions at symbol name and assignment tab stops
        const int               k_MaxCount      = DisplayData.MaxItems;
        static int              s_SharedCount;                                                      // | Note no "= 0". All memory is zero'd out by default, so do not redundantly assign.
        int                     m_CurrentCount;

        public int totalCount = 123;                                                                // | Public fields are camelCase with no prefix

        public string defaultName { get { return Environment.MachineName; } }                       // | Public properties are camelCase with no prefix

        [Example]                                                                                   // | Drop 'Attribute' postfix when applying an attribute
        public int currentCount
        {
            get { return m_CurrentCount; }                                                          // | Getters are always trivial and do not mutate state (this includes first-run cached results); use a full method if you want to do calculations or caching
            set { m_CurrentCount = value; }
        }                                                                                           // | Put exactly one blank line between multi-line methods and properties

        public string description
        {
            get                                                                                     // | For multiline method bodies, the 'get' and 'set' keywords must be on their own line
            {
                return string.Format(
                    "shared: {0}\ncurrent: {1}\n",
                    s_SharedCount, m_CurrentCount);
            }
        }

        [Description("I do things"), DebuggerNonUserCode]                                           // | Attributes always go on a line separate from what they apply to (unless a parameter), and joining them is encouraged if they are short
        public void DoThings(IEnumerable<IThingAgent> thingsToDo, string propertyDescription)       // | For types that are already internal (like class Example), use public instead of internal for members and nested types
        {
            var doneThings = new List<IThingAgent>();                                               // | 'var' required on any 'new' where the type we want is the same as what is being constructed
            var indent = new string(' ', 4);                                                        // | ...even primitive types
                                                                                                    // | When appropriate, separate code blocks by a single empty line
            IList<string> doneDescriptions = new List<string>();                                    // | (This is a case where 'var' not required because the types of the variable vs the ctor are different)

            foreach (var thingToDo in thingsToDo)                                                   // | 'var' required in all foreach
            {
                if (!thingToDo.DoThing(propertyDescription, m_CurrentCount))
                    break;                                                                          // | Braces not required for single statements under if or else, but that single statement must be on its own line

                using (File.CreateText(@"path\to\something.txt"))                                   // | Use @"" style string literal for paths with backslashes and regular expression patterns
                {
                    using (new ComputeBuffer(10, 20))                                               // | Braces required for nested using's until tooling fixed to support nesting them without indentation (previous rule on hold: 'Drop braces on outer levels of nested using's and keep indent the same')
                    {                                                                               // | Braces required for deepest level of nested using's
                        doneThings.Add(thingToDo);
                    }
                }
            }

            foreach (var doneThing in doneThings)                                                   // | Dirty details about allocs at https://q.unity3d.com/questions/1465/when-does-using-foreach-in-c-cause-an-allocation.html
            {                                                                                       // | Braces are required for loops (foreach, for, while, do) as well as 'fixed' and 'lock'
                doneDescriptions.Add(doneThing.operationDescription);
                Debug.Log(indent + "Doing thing: " + doneThing.operationDescription);               // | Prefer a + b + c over string.Concat(a, b, c)
            }

            Debug.Log("System Object is " + typeof(object));                                        // | Always use lowercase `object` for the System.Object class.
            Debug.Log("Unity Object is " + typeof(UnityEngine.Object));                             // | Always use a fully qualified name for Unity's Object type, and never 'Object'
        }

        public void ControlFlow(string message, object someFoo, WindingOrder windingOrder)          // | Use c# aliases of System types (e.g. object instead of Object, float instead of Single, etc.)
        {
            for (int i = 0; i < k_MaxCount; ++i)                                                    // | Using i and j for trivial local iterators is encouraged
            {
                // all of this is nonsense, and is just meant to demonstrate formatting             // | Place comments about multiple lines of code directly above them, with one empty line above the comment to visually group it with its code
                if ((i % -3) - 1 == 0)                                                              // | Wrap parens around subexpressions is optional but recommended to make operator precedence clear
                {
                    ++m_CurrentCount;
                    s_SharedCount *= (int)k_DefaultLength.x + totalCount;

                    do                                                                              // | 'while', 'do', 'for', 'foreach', 'switch' are always on a separate line from the code block they control
                    {
                        i += s_SharedCount;
                    }
                    while (i < m_CurrentCount);
                }
                else                                                                                // | 'else' always at same indentation level as its 'if'
                {
                    Debug.LogWarning("Skipping over " + i);                                         // | Drop 'ToString()' when not required by compiler
                    goto skip;                                                                      // | Goto's not necessarily considered harmful, not disallowed, but should be scrutinized for utility before usage
                }
            }

        skip:                                                                                       // | Goto label targets un-indented from parent scope one tab stop

            // more nonsense code for demo purposes
            switch (windingOrder)
            {
                case WindingOrder.Clockwise:                                                        // | Case labels indented under switch
                case WindingOrder.CounterClockwise:                                                 // | Braces optional if not needed for scope (but note indentation of braces and contents)
                    if (s_SharedCount == DisplayData.MaxItems)                                      // | Constants go on the right in comparisons (do not follow 'yoda' style)
                    {
                        var warningDetails = someFoo.ToString();                                    // | 'var' for the result of assignments is optional (either way, good variable naming is most important)
                        for (var i = 0; i < s_SharedCount; ++i)
                        {
                            Debug.LogWarning("Spinning a " + warningDetails);
                        }
                    }
                    break;                                                                          // | 'break' inside case braces, if any

                case WindingOrder.Charm:
                    Debug.LogWarning("Check quark");                                                // | Indentation is the same, with or without scope braces
                    break;

                case WindingOrder.Singularity:
                {
                    var warningDetails = message;                                                   // | (this seemingly pointless variable is here solely to require braces on the case statements and show the required formatting)

                    if (message == Registry.ClassesRoot.ToString())
                    {
                        // Already correct so we don't need to do anything here                     // | Empty blocks should (a) only be used when it helps readability, (b) always use empty braces (never a standalone semicolon), and (c) be commented as to why the empty block is there
                    }
                    else if (m_CurrentCount > 3)
                    {
                        if (s_SharedCount < 10)                                                     // | Braces can only be omitted at the deepest level of nested code
                            Debug.LogWarning("Singularity! (" + warningDetails + ")");
                    }
                    else if (s_SharedCount > 5)                                                     // | 'else if' always on same line together
                        throw new IndexOutOfRangeException();
                    else if ((s_SharedCount > 7 && m_CurrentCount != 0) || message == null)         // | Always wrap subexpressions in parens when peer precedence is close enough to be ambiguous (e.g. && and || are commonly confused)
                        throw new NotImplementedException();

                    break;
                }

                default:
                    throw new InvalidOperationException("What's a " + windingOrder + "?");
            }
        }

        // |[Parameterized Types]
        // |    - When only a single parameterized type is used, naming it 'T' is acceptable.
        // |    - For more than one parameterized type, use descriptive names prefixed with 'T'.
        // |    - Consider indicating constraints placed on a type parameter in the name of the parameter.
        public static TResult Transmogrify<TResult, TComponent>(                                    // | When wrapping params, do not leave any on line with function name
            TComponent component, Func<TComponent, TResult> converter)                              // | When wrapping, only indent one stop (do not line up with paren)
            where TComponent : Component
        {
            return converter(component);
        }
    }

    // |[Structs]
    // |    - Structs are very special. See the data-oriented programming guidelines before creating one (http://confluence.hq.unity3d.com/x/L4O3).
    // |    - Name classes and structs with nouns or noun phrases.
    // |    - No prefix on class names (no 'C' or 'S' etc.).
    struct MethodQuery
    {
        public string            name;                                                              // | Data in structs can only be public fields, and must use camelCase naming with no prefix
        public IEnumerable<Type> paramTypes;
        public Type              returnType;

        public override string ToString()                                                           // | Methods generally are not permitted in structs, with exceptions like this noted in the data-oriented programming guidelines.
        {
            var paramTypeNames = paramTypes                                                         // | Prefer fluent function call syntax over LINQ syntax (i.e. y.Select(x => z) instead of 'from x in y select z')
                .Select(p => p.ToString())                                                          // | Prefer breaking long fluent operator chains into one line per operator
                .Where(p => p.Length > 2)
                .OrderBy(p => p[0])
                .ToArray();

            return string.Format(
                "{0} {1}({2})",
                returnType, name, string.Join(", ", paramTypeNames));
        }
    }

    // |[Attributes]
    // |    - Mark up all attributes with an AttributeUsage, as narrow as possible.
    // |    - Postfix attribute class names with "Attribute".
    [AttributeUsage(AttributeTargets.Property)]
    public class ExampleAttribute : Attribute
    {                                                                                               // | Empty types have braces on their own lines
    }

    // |[Exceptions]
    // |    - Postfix exception class names with "Exception".
    // |    - Do not inherit from ApplicationException (see http://stackoverflow.com/a/5685943/14582).
    public class ExampleException : Exception
    {
        public ExampleException() {}
        public ExampleException(string message) : base(message) {}
        public ExampleException(string message, Exception innerException) : base(message, innerException) {}
    }
}

// (this stuff is just here to demo some rules above)

namespace Company.BuildSystem
{
    public static class DisplayData
    {
        public const int MaxItems = 100;
    }
}
