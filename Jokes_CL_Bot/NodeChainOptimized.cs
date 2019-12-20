using System;
using System.Collections.Generic;
using System.Linq;

namespace AnekParser
{
    public class NodeChainOptimized
    {
        public string[] Keys { get; set; }
        public Node[] Values { get; set; }

        private NodeChain original;
        
        public NodeChainOptimized()
        {
        }

        public NodeChainOptimized(NodeChain nc)
        {
            original = nc;
            Keys = nc.Nodes.Keys.OrderBy(s => s).ToArray();
            Values = Keys.Select(k => nc.Nodes[k]).ToArray();
        }

        public void Print(int x)
        {
            for (var i = 0; i < x; i++)
                Console.WriteLine($"[{Keys[i]}] = {Values[i].Count.Count}\n{original.Nodes[Keys[i]].Count.Count}");
        }
        
        public Node this[string key]
        {
            get
            {
                var index = Array.BinarySearch(Keys, key);
                return index >= 0 ? Values[index] : null;
            }
        }

        private bool ContainsKey(string key)
        {
            return this[key] != null;
        }

        private string MakeSentence(Random r, int number)
        {
            var list = new List<string>();
            var word0 = "";
            var word1 = this[word0].GetNext(r);
            while (number > 0)
            {
                //Console.WriteLine($"-{word0} {word1}-");
                var t = !ContainsKey($"{word0} {word1}") ? (ContainsKey(word1) ? this[word1].GetNext(r) : "<error>") : this[$"{word0} {word1}"].GetNext(r);
                word0 = word1;
                word1 = t;
                if (word0 == "")
                    break;
                if (word0 == ".")
                    number--;
                list.Add(word0);
            }

            var sentence = string.Join(" ", list.Select(x => x.Split("$")[0]));
            return $"{sentence[0].ToString().ToUpper()}{sentence.Substring(1)} ";
        }

        public string MakeText(int number, Random r)
        {
            return MakeSentence(r, number).Replace(" .", ".");
        }
    }
}