using System.Collections.Concurrent;

namespace Demo;

public static class LoginTracker
{
    // Dictionary to store failed attempts: <Email, FailedAttempts>
    public static ConcurrentDictionary<string, int> FailedLoginAttempts = new ConcurrentDictionary<string, int>();

    // Dictionary to store blocked users: <Email, UnblockTime>
    public static ConcurrentDictionary<string, DateTime> BlockedUsers = new ConcurrentDictionary<string, DateTime>();
}