#
# DeepLingo compiler - Project make file: 
#    Esteban Gil Martinez        A01375048
#    Javier Esponda Hernández    A01374645
#    Joel Lara Quintana          A01374649
#  Copyright (C) 2018, Error 404 NullPointeException, ITESM CEM
#  
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#  
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#  
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
#

buttercup.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs               
	mcs -out:deepLingo.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs
			
clean:
	rm deepLingo.exe