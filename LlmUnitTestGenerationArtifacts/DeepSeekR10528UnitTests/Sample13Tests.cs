using Dataset.Sample13;
using NSubstitute;

namespace DeepSeekR10528UnitTests;

public class OntologyHelperTests
{
    [Theory]
    [InlineData("http://example.com#Resource", "Resource")]
    [InlineData("http%3a%2f%2fexample.com%23Resource", "Resource")]
    [InlineData("#OnlyFragment", "OnlyFragment")]
    [InlineData("http://example.com#", "")]
    [InlineData("abc#def#ghi", "def#ghi")]
    [InlineData("no#hash", "no#hash")]
    [InlineData("", "")]
    public void ParseNode_UriNode_ReturnsExpectedStringAfterUnEscaping(string input, string expected)
    {
        // Arrange
        var node = Substitute.For<INode>();
        node.NodeType.Returns(NodeType.Uri);
        node.ToString().Returns(input);

        // Act
        var result = node.ParseNode();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("value^type", "value")]
    [InlineData("value^^type", "value")]
    [InlineData("^type", "")]
    [InlineData("v^", "v")]
    [InlineData("abc^def^ghi", "abc")]
    [InlineData("hello%20world%5Erest", "hello world")]
    public void ParseNode_LiteralNode_WithCaret_ReturnsExpectedStringAfterUnEscaping(string input, string expected)
    {
        // Arrange
        var node = Substitute.For<INode>();
        node.NodeType.Returns(NodeType.Literal);
        node.ToString().Returns(input);

        // Act
        var result = node.ParseNode();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("no caret in string")]
    [InlineData("")]
    [InlineData(null)]
    public void ParseNode_LiteralNode_WithoutCaret_ThrowsArgumentOutOfRangeException(string input)
    {
        // Arrange
        var node = Substitute.For<INode>();
        node.NodeType.Returns(NodeType.Literal);
        node.ToString().Returns(input);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => node.ParseNode());
    }

    [Theory]
    [InlineData(NodeType.Blank)]
    [InlineData(NodeType.GraphLiteral)]
    [InlineData(NodeType.Variable)]
    [InlineData(NodeType.Triple)]
    public void ParseNode_NodeTypeNotUriOrLiteral_ReturnsEmptyString(NodeType nodeType)
    {
        // Arrange
        var node = Substitute.For<INode>();
        node.NodeType.Returns(nodeType);
        node.ToString().Returns("any string");

        // Act
        var result = node.ParseNode();

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
