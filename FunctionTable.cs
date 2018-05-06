/*
  DeepLingo compiler - Function table class.
    Esteban Gil Martinez        A01375048
    Javier Esponda Hernández    A01374645
    Joel Lara Quintana          A01374649
  
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {
    public class FunctionTable: IEnumerable<KeyValuePair<string, int>> {
        IDictionary<string, int> data = new SortedDictionary<string, int>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Function Table\n");
            sb.Append("====================\n");
            foreach (var entry in data) {
                sb.Append(String.Format("{0}: {1}\n", entry.Key, entry.Value));
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public int this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
