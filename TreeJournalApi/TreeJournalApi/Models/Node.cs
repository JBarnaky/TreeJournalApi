public class Node
{
    public int Id { get; set; }
    public string Name { get; set; } // Mandatory field
    public int? ParentId { get; set; } // Optional field for parent node
    public ICollection<Node> Children { get; set; } // Navigation property
}
