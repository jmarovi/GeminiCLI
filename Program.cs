using System.Text;
using System.Text.Json;

namespace GeminiCLI;

class Program {
    // API key
    static readonly string key = "YOUR_API_KEY_HERE";

    // Endpoint for the model
    static readonly string modelEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={key}";

    // The chat history
    static readonly ChatHistory chatHistory = new() { contents = [] };

    // Main function
    public static async Task Main() {
        // Chat loop
        while (true) {
            // Get user input
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("User: ");
            Console.ForegroundColor = ConsoleColor.White;
            string? message = Console.ReadLine();

            // if input is exit break out of the loop
            if (message?.ToLower() == "exit") break;

            // if input is null do nothing
            if (message == null) continue;

            // Add the user's message to the chat history
            AddToChatHistory(message, role: "user");

            // Serialize the whole chat history
            string? jsonBody = await Serialize(chatHistory);

            // Write the model's response
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Gemini: ");
            Console.ForegroundColor = ConsoleColor.White;

            // Send request and get response
            string? modelResponse = await GetModelResponse(jsonBody);

            // Deserialize the response with GetModelResponse and extract the text from the response
            string? responseText = await ExtractTextFromResponse(modelResponse);

            // Add the model's response to the chat history
            AddToChatHistory(responseText, role: "model");

            // Write the model's response
            await OutputResponse(responseText);
        }
    }

    // Function to output the response
    private static async Task OutputResponse(string responseText) {
        // split the response words by spaces
        string[] responseWords = responseText.Split(' ');

        // for each word in the response
        foreach (string word in responseWords) {

            // if the remaining console length is less than the size of the response, add a new line
            if (Console.WindowWidth - Console.CursorLeft < word.Length) {
                Console.WriteLine();
            }

            // write the word to the console
            Console.Write(value: word + " ");

            // wait 50 milliseconds to simulate typing
            await Task.Delay(millisecondsDelay: 50);
        }
        // add a new line
        Console.WriteLine();
    }

    // Function to serialize the chat history
    private static async Task<string> Serialize(ChatHistory chatHistory) {
        // Variable for the chat history in JSON format as a string
        string jsonBody = JsonSerializer.Serialize(chatHistory, options: new JsonSerializerOptions {
            WriteIndented = true
        });

        // return the json body
        return jsonBody;
    }

    // Function to add a message to the chat history
    private static void AddToChatHistory(string? message, string role) {
        // Add the message to the chat history Content list
        chatHistory.contents?.Add(
            item: new Content {
                role = role,
                parts =
                [
                    new Part { text = message }
                ]
            }
        );
    }

    // Function to extract the text from the model's response
    private static async Task<string> ExtractTextFromResponse(string modelResponse) {
        // Parse the model's response
        using JsonDocument doc = JsonDocument.Parse(json: modelResponse);
        // Extract the text from the model's response
        JsonElement root = doc.RootElement;
        // Extract the candidates from the model's response
        JsonElement candidates = root.GetProperty(propertyName: "candidates");
        // Extract the first candidate from the model's response
        JsonElement candidate = candidates[index: 0];
        // Extract the content from the candidate
        JsonElement content = candidate.GetProperty(propertyName: "content");
        // Extract the parts from the content
        JsonElement parts = content.GetProperty(propertyName: "parts");
        // Extract the first part from the parts
        JsonElement part = parts[index: 0];
        // Extract the text from the part
        string text = part.GetProperty(propertyName: "text").GetString();
        // Return the text
        return text;
    }

    // Function to get the model's response
    private static async Task<string> GetModelResponse(string jsonBody) {
        // Create a new HttpClient
        HttpClient? client = new();
        // Create a new HttpRequestMessage
        HttpRequestMessage? request = new() {
            // Set the request URI and method
            RequestUri = new Uri(modelEndpoint),
            Method = HttpMethod.Post
        };
        // Add the Accept header
        request.Headers.Add("Accept", "*/*");
        // Set the body of the request
        string? bodyString = jsonBody;
        // Create a new StringContent with the body string and the correct content type
        StringContent? content = new(content: bodyString, encoding: Encoding.UTF8, mediaType: "application/json");
        // Set the content of the request
        request.Content = content;
        // Send request and get response
        HttpResponseMessage? response = await client.SendAsync(request);
        // Get the response content as a string
        string? result = await response.Content.ReadAsStringAsync();
        // Return the result
        return result;
    }
}
