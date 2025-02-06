using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;


var pathConfig = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
var config = new ConfigurationBuilder()
            .AddJsonFile(pathConfig, optional: false, reloadOnChange: true)
            .Build();

var builder = Kernel.CreateBuilder();

builder.AddOpenAIChatCompletion("gpt-4o-mini", config["gpt.secreet"] ?? "");

Kernel kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory chatHistory = new ChatHistory();


while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("User>> ");

    var userMessage = Console.ReadLine();

    chatHistory.AddUserMessage(userMessage);

    var response = chatService.GetStreamingChatMessageContentsAsync(chatHistory, kernel: kernel);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("AI>> ");

    string fullMessage = "";

    await foreach (var chat in response)
    {
        Console.Write(chat);
        fullMessage += chat;
    }

    Console.WriteLine();

    chatHistory.AddAssistantMessage(fullMessage);
}