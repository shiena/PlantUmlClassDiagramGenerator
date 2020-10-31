using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlantUmlClassDiagramGenerator.Library
{
    public static class CSharpSyntaxNodeExtension
    {
        private const string NAMESPACE_CLASS_DELIMITER = ".";
        public const string NESTED_CLASS_DELIMITER = "+";
        private const char TYPEPARAMETER_CLASS_DELIMITER = '`';

        public static string GetFullName(this CSharpSyntaxNode source)
        {
            var namespaces = new LinkedList<NamespaceDeclarationSyntax>();
            var types = new LinkedList<TypeDeclarationSyntax>();
            for (var parent = source.Parent; parent != null; parent = parent.Parent)
            {
                if (parent is NamespaceDeclarationSyntax ns)
                {
                    namespaces.AddFirst(ns);
                }
                else if (parent is TypeDeclarationSyntax type)
                {
                    types.AddFirst(type);
                }
            }

            var result = new StringBuilder();
            foreach (var item in namespaces)
            {
                result.Append(item.Name).Append(NAMESPACE_CLASS_DELIMITER);
            }

            foreach (var type in types)
            {
                AppendName(result, type);
                result.Append(NESTED_CLASS_DELIMITER);
            }

            AppendName(result, source);

            return result.ToString();
        }

        private static void AppendName(StringBuilder builder, CSharpSyntaxNode cSharpSyntaxNode)
        {
            if (cSharpSyntaxNode is BaseTypeDeclarationSyntax baseType)
            {
                builder.Append(baseType.Identifier.Text);
            }
            else if (cSharpSyntaxNode is SimpleNameSyntax simpleName)
            {
                builder.Append(simpleName.Identifier.Text);
            }
            if (!(cSharpSyntaxNode is TypeDeclarationSyntax type)) return;
            var typeArguments = type.TypeParameterList?.ChildNodes()
                .Count(node => node is TypeParameterSyntax) ?? 0;
            if (typeArguments > 0)
            {
                builder.Append(TYPEPARAMETER_CLASS_DELIMITER).Append(typeArguments);
            }
        }
    }
}