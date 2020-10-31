using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlantUmlClassDiagramGenerator.Library
{
    public class RelationshipCollection : IEnumerable<Relationship>
    {
        private IList<Relationship> _items = new List<Relationship>();

        public void AddInheritanceFrom(TypeDeclarationSyntax syntax)
        {
            if (syntax.BaseList == null) return;

            var subTypeName = TypeNameText.From(syntax);
            if (subTypeName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                subTypeName.Identifier = $"\"{subTypeName.Identifier}\"";
            }

            foreach (var typeStntax in syntax.BaseList.Types)
            {
                var typeNameSyntax = typeStntax.Type as SimpleNameSyntax;
                if (typeNameSyntax == null) continue;
                var baseTypeName = TypeNameText.From(typeNameSyntax);
                if (baseTypeName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
                {
                    baseTypeName.Identifier = $"\"{baseTypeName.Identifier}\"";
                }
                _items.Add(new Relationship(baseTypeName, subTypeName, "<|--", baseTypeName.TypeArguments));
            }
        }

        public void AddInnerclassRelationFrom(SyntaxNode node)
        {
            var outerTypeNode = node.Parent as BaseTypeDeclarationSyntax;
            var innerTypeNode = node as BaseTypeDeclarationSyntax;

            if (outerTypeNode == null || innerTypeNode == null) return;

            var outerTypeName = TypeNameText.From(outerTypeNode);
            if (outerTypeName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                outerTypeName.Identifier = $"\"{outerTypeName.Identifier}\"";
            }
            var innerTypeName = TypeNameText.From(innerTypeNode);
            if (innerTypeName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                innerTypeName.Identifier = $"\"{innerTypeName.Identifier}\"";
            }
            _items.Add(new Relationship(outerTypeName, innerTypeName, "+--"));
        }

        public void AddAssociationFrom(FieldDeclarationSyntax node, VariableDeclaratorSyntax field)
        {
            var baseNode = node.Declaration.Type as SimpleNameSyntax;
            var subNode = node.Parent as BaseTypeDeclarationSyntax;

            if (baseNode == null || subNode == null) return;

            var symbol = field.Initializer == null ? "-->" : "o->";

            var baseName = TypeNameText.From(baseNode);
            if (baseName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                baseName.Identifier = $"\"{baseName.Identifier}\"";
            }
            var subName = TypeNameText.From(subNode);
            if (subName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                subName.Identifier = $"\"{subName.Identifier}\"";
            }
            _items.Add(new Relationship(subName, baseName, symbol, "", field.Identifier.ToString() + baseName.TypeArguments));
        }

        public void AddAssociationFrom(PropertyDeclarationSyntax node)
        {
            var baseNode = node.Type as SimpleNameSyntax;
            var subNode = node.Parent as BaseTypeDeclarationSyntax;

            if (baseNode == null || subNode == null) return;

            var symbol = node.Initializer == null ? "-->" : "o->";

            var baseName = TypeNameText.From(baseNode);
            if (baseName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                baseName.Identifier = $"\"{baseName.Identifier}\"";
            }
            var subName = TypeNameText.From(subNode);
            if (subName.Identifier.Contains(CSharpSyntaxNodeExtension.NESTED_CLASS_DELIMITER))
            {
                subName.Identifier = $"\"{subName.Identifier}\"";
            }
            _items.Add(new Relationship(subName, baseName, symbol, "", node.Identifier.ToString() + baseName.TypeArguments));
        }

        public IEnumerator<Relationship> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
