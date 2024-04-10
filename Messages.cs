namespace GeminiCLI;
public class Part {
    public string? text { get; set; }
}

public class Content {
    public string? role { get; set; }
    public List<Part>? parts { get; set; }
}

public class ChatHistory {
    public List<Content>? contents { get; set; }
}

public partial class Response {
    public Candidate[] candidates { get; set; }
}

public partial class Candidate {
    public Content Contents { get; set; }
    public string FinishReason { get; set; }
    public long Index { get; set; }
    public SafetyRating[] SafetyRatings { get; set; }
}


public partial class SafetyRating {
    public string Category { get; set; }
    public string Probability { get; set; }
}
