using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public static class SessionManager
    {
        private static readonly Dictionary<string, List<string>> ActiveSessions = new Dictionary<string, List<string>>();

        public static void AddSession(string userId, string sessionId)
        {
            if (!ActiveSessions.ContainsKey(userId))
            {
                ActiveSessions[userId] = new List<string>();
            }

            ActiveSessions[userId].Add(sessionId);
        }

        public static void RemoveSession(string userId, string sessionId)
        {
            if (ActiveSessions.ContainsKey(userId))
            {
                ActiveSessions[userId].Remove(sessionId);

                if (ActiveSessions[userId].Count == 0)
                {
                    ActiveSessions.Remove(userId);
                }
            }
        }

        public static List<string> GetActiveSessions(string userId)
        {
            return ActiveSessions.ContainsKey(userId) ? ActiveSessions[userId] : new List<string>();
        }
    }
    }
