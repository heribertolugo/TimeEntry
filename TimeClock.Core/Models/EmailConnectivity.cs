namespace TimeClock.Core.Models;

public record struct EmailConnectivity(string Email, string Server, string Password, bool UseSsl, int Port);