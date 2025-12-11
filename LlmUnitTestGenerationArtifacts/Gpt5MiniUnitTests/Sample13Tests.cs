using Dataset.Sample13;
using NSubstitute;

namespace Gpt5MiniUnitTests
{
    public class OntologyHelperTests
    {
        [Fact]
        public void ParseNode_NodeTypeUri_WithHash_ReturnsFragment()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Uri);
            node.ToString().Returns("http://example.org#Fragment");

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal("Fragment", result);
        }

        [Fact]
        public void ParseNode_NodeTypeUri_NoHash_ReturnsFullString()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Uri);
            node.ToString().Returns("nohash");

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal("nohash", result);
        }

        [Fact]
        public void ParseNode_NodeTypeUri_NullToString_ReturnsEmptyString()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Uri);
            node.ToString().Returns((string?)null);

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ParseNode_NodeTypeUri_HashAtEnd_ReturnsEmptyString()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Uri);
            node.ToString().Returns("http://example.org#");

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ParseNode_NodeTypeLiteral_WithCaret_ReturnsValueBeforeCaret()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Literal);
            node.ToString().Returns("value^^xsd:string");

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal("value", result);
        }

        [Fact]
        public void ParseNode_NodeTypeLiteral_CaretAtStart_ReturnsEmptyString()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Literal);
            node.ToString().Returns("^rest");

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ParseNode_NodeTypeLiteral_NoCaret_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Literal);
            node.ToString().Returns("noCaretHere");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => node.ParseNode());
        }

        [Fact]
        public void ParseNode_NodeTypeLiteral_NullToString_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(NodeType.Literal);
            node.ToString().Returns((string?)null);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => node.ParseNode());
        }

        [Theory]
        [InlineData(NodeType.Blank)]
        [InlineData(NodeType.GraphLiteral)]
        [InlineData(NodeType.Variable)]
        [InlineData(NodeType.Triple)]
        public void ParseNode_OtherNodeTypes_ReturnsEmptyString(NodeType type)
        {
            // Arrange
            var node = Substitute.For<INode>();
            node.NodeType.Returns(type);
            node.ToString().Returns("irrelevant");

            // Act
            var result = node.ParseNode();

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}
