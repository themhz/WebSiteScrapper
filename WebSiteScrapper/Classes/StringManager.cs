using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteScrapper.Classes
{
    public class StringManager
    {


        public double CompareStrings(string s1, string s2)
        {
            if (String.IsNullOrEmpty(s1) || String.IsNullOrEmpty(s2))
            {
                return 0;
            }

            int matches = 0;

            for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
            {
                if (s1[i] == s2[i])
                {
                    matches++;
                }
            }
            return (double)matches / Math.Max(s1.Length, s2.Length);
        }

        public string[] GetStringDifference(string str1, string str2)
        {
            // Create a list to store the results
            List<string> differences = new List<string>();

            // Iterate over each character in both strings
            for (int i = 0; i < Math.Max(str1.Length, str2.Length); i++)
            {
                // Get the current character in string 1
                char c1 = str1[i];

                // Get the current character in string 2
                char c2 = str2[i];

                // Check if the characters are different
                if (c1 != c2)
                {
                    // Add the difference to the list
                    differences.Add(String.Format("{0} {1} {2}", c1, (c1 < c2) ? "<" : ">", c2));
                }
            }
            // Return the list as an array
            return differences.ToArray();
        }


        //This is the Levenshtein distance algorithm 
        public double CalculateStringDifference(string s1, string s2)
        {
            int[,] dp = new int[s1.Length + 1, s2.Length + 1];
            for (int i = 0; i <= s1.Length; i++)
            {
                dp[i, 0] = i;
            }
            for (int j = 0; j <= s2.Length; j++)
            {
                dp[0, j] = j;
            }
            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }
            int maxLen = Math.Max(s1.Length, s2.Length);
            double difference = 1.0 - ((double)dp[s1.Length, s2.Length] / maxLen);
            return difference;
        }


        public string[] CompareTextFiles(string path1, string path2)
        {
            string[] lines1 = File.ReadAllLines(path1);
            string[] lines2 = File.ReadAllLines(path2);

            int[,] dp = new int[lines1.Length + 1, lines2.Length + 1];
            for (int i = 0; i <= lines1.Length; i++)
            {
                dp[i, 0] = i;
            }
            for (int j = 0; j <= lines2.Length; j++)
            {
                dp[0, j] = j;
            }
            for (int i = 1; i <= lines1.Length; i++)
            {
                for (int j = 1; j <= lines2.Length; j++)
                {
                    int cost = (lines1[i - 1] == lines2[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }

            List<string> differences = new List<string>();
            int x = lines1.Length;
            int y = lines2.Length;
            while (x > 0 || y > 0)
            {
                int insert = (x > 0) ? dp[x - 1, y] : int.MaxValue;
                int delete = (y > 0) ? dp[x, y - 1] : int.MaxValue;
                int replace = (x > 0 && y > 0) ? dp[x - 1, y - 1] : int.MaxValue;

                int min = Math.Min(Math.Min(insert, delete), replace);

                if (min == replace)
                {
                    if (dp[x, y] != dp[x - 1, y - 1])
                    {
                        differences.Add($"Line {x}: Replace '{lines1[x - 1]}' with '{lines2[y - 1]}'");
                    }
                    x--;
                    y--;
                }
                else if (min == delete)
                {
                    differences.Add($"Line {x}: Delete '{lines1[x - 1]}'");
                    x--;
                }
                else if (min == insert)
                {
                    differences.Add($"Line {x + 1}: Insert '{lines2[y - 1]}'");
                    y--;
                }
            }

            differences.Reverse();
            return differences.ToArray();
        }

    }
}
