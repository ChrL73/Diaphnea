using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IComparerTest
{
    class IComparerTestMain
    {
        static void Main(string[] args)
        {
            Text text1 = new Text();
            text1.setText("la_francais", "manger");

            Text text2 = new Text();
            text2.setText("la_francais", "jouer");
            text2.setText("la_english", "play");

            Text text3 = new Text();
            text3.setText("la_francais", "faire");
            text3.setText("la_english", "make");

            Text text4 = new Text();
            text4.setText("la_francais", "faire");
            text4.setText("la_english", "do");

            TestComparer comparer = new TestComparer();
            SortedDictionary<Text, int> dictionary = new SortedDictionary<Text, int>(comparer);

            dictionary.Add(text1, 1);
            dictionary.Add(text2, 2);
            dictionary.Add(text3, 3); 
            dictionary.Add(text4, 4); // Exception: comparer.Compare(text3, text4) == 0
        }
    }
}
