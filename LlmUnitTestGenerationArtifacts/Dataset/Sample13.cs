namespace Dataset.Sample13;

public static class OntologyHelper
{
    public static string ParseNode(this INode node)
    {
        switch (node.NodeType)
        {
            case NodeType.Uri:
                return node.ParseUriNode();
            case NodeType.Literal:
                return node.ParseLiteralNode();
            default:
                return string.Empty;
        }
    }

    private static string ParseUriNode(this INode node)
    {
        var sparqlResultString = Uri.UnescapeDataString(node.ToString() ?? string.Empty);
        var separatorIndex = sparqlResultString.IndexOf('#');
        var result = sparqlResultString.Substring(separatorIndex + 1);

        return result;
    }

    private static string ParseLiteralNode(this INode node)
    {
        var sparqlResultString = Uri.UnescapeDataString(node.ToString() ?? string.Empty);
        var separatorIndex = sparqlResultString.IndexOf('^');
        var result = sparqlResultString.Substring(0, separatorIndex);

        return result;
    }
}

public interface INode
{
    NodeType NodeType
    {
        get;
    }

    string ToString();
}

public enum NodeType
{
    Blank,
    Uri,
    Literal,
    GraphLiteral,
    Variable,
    Triple,
}
