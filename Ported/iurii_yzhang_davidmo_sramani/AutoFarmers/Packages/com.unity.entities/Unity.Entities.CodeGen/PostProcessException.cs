using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;

namespace Unity.Entities.CodeGen
{
    class FoundErrorInUserCodeException : Exception
    {
        public DiagnosticMessage[] DiagnosticMessages { get; }

        public FoundErrorInUserCodeException(DiagnosticMessage[] diagnosticMessages)
        {
            DiagnosticMessages = diagnosticMessages;
        }

        public  override string ToString() => DiagnosticMessages.Select(dm=>dm.MessageData).SeparateByComma();
    }

    class PostProcessException : Exception
    {
        public string MessageWithoutLocation { get; }
        public MethodDefinition Method { get; }
        public Instruction Instruction { get; }
            
        public PostProcessException(string messageWithoutLocation, MethodDefinition method = null, Instruction instruction = null)
        {
            MessageWithoutLocation = messageWithoutLocation;
            Method = method;
            Instruction = instruction;
        }

        public override string Message => MessageWithoutLocation;

        public SequencePoint SequencePointFor(MethodDefinition context = null)
        {
            SequencePoint seq = null;

            if (Instruction != null)
                seq = CecilHelpers.FindBestSequencePointFor(Method ?? context, Instruction);

            if (seq == null && Method != null)
                seq = Method.DebugInformation.SequencePoints.FirstOrDefault();

            return seq;
        }

        public DiagnosticMessage ToDiagnosticMessage(MethodDefinition context)
        {
            var result = new DiagnosticMessage()
            {
                DiagnosticType = DiagnosticType.Error,
                MessageData = MessageWithoutLocation,
            };
            
            var seq = SequencePointFor(context);
            if (seq != null)
            {
                result.Column = seq.StartColumn;
                result.Line = seq.StartLine;
            }
            return result;
        }
    }
}