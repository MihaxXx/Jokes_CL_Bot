using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AnekParser
{
    public struct Pair
    {
        public string value1 { get; set; }
        public string value2 { get; set; }

        public Pair(string v1, string v2)
        {
            value1 = v1;
            value2 = v2;
        }
    }

    public class NodeChain
    {
        public Dictionary<string, Node> Nodes { get; set; } = new Dictionary<string, Node>();

        public Dictionary<string, Node> BiNodes { get; set; } = new Dictionary<string, Node>();
        
        private void ParseCombination(string from1, string from2, string to)
        {
            if (!BiNodes.ContainsKey($"{from1} {from2}"))
                BiNodes[$"{from1} {from2}"] = new Node();
            BiNodes[$"{from1} {from2}"].AddWord(to);
        }
        
        private void ParseCombination(string from, string to)
        {
            if (!Nodes.ContainsKey(from))
                Nodes[from] = new Node();
            Nodes[from].AddWord(to);
        }

        private void ParseSentence(List<string> sentence)
        {
            if (sentence.Count <= 1)
                return;
            ParseCombination("", sentence[0]);
            ParseCombination("", sentence[0], sentence[1]);
            for (var i = 0; i < sentence.Count - 2; i++)
            {
                ParseCombination(sentence[i], sentence[i + 1]);
                ParseCombination(sentence[i],sentence[i + 1], sentence[i + 2]);
            }

            ParseCombination(sentence[sentence.Count - 2], sentence[sentence.Count - 1]);
            ParseCombination(sentence[sentence.Count - 1],"");
        }

        public void ParseText(List<List<string>> text, List<string> lemmas)
        {
            text.ForEach(l => l.Add("."));
            //int i = 0;
            var sentence = text.SelectMany(x => x).Select(x =>
            {
                return x;
                //if (x == ".") return ".";
                //return $"{x}${lemmas[i++]}";
            }).ToList();
            ParseSentence(sentence);
        }

        private string MakeSentence(Random r)
        {
            var list = new List<string>();
            var word0 = "";
            var word1 = Nodes[word0].GetNext(r);
            while (true)
            {
                //Console.WriteLine($"{word0} {word1}");
                string t;
                t = !BiNodes.ContainsKey($"{word0} {word1}") ? Nodes[word1].GetNext(r) : BiNodes[$"{word0} {word1}"].GetNext(r);
                word0 = word1;//Nodes[word].GetNext(r);
                word1 = t;
                if (word1 == "")
                    break;
                list.Add(word0);
            }

            var sentence = string.Join(" ", list.Select(x => x.Split("$")[0]));
            return $"{sentence[0].ToString().ToUpper()}{sentence.Substring(1)}";
        }

        public string MakeText(int number, Random r = null)
        {
            if (r == null)
                r = new Random();
            return string.Join("", Enumerable.Range(1, number).Select(x => MakeSentence(r)));
        }
    }
}