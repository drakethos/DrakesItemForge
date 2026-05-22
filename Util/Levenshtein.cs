using System;
using System.Collections.Generic;

namespace DrakesItemForge.Util;

internal static class Levenshtein
{
    public static int Distance(string a, string b)
    {
        if (string.IsNullOrEmpty(a))
            return b?.Length ?? 0;
        if (string.IsNullOrEmpty(b))
            return a.Length;

        int n = a.Length;
        int m = b.Length;
        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++)
            d[i, 0] = i;
        for (int j = 0; j <= m; j++)
            d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    public static string? FindClosest(string input, IEnumerable<string> candidates, int maxDistance = 3)
    {
        string? best = null;
        int bestDist = int.MaxValue;

        foreach (var c in candidates)
        {
            int dist = Distance(input, c);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = c;
            }
        }

        return bestDist <= maxDistance ? best : null;
    }
}
