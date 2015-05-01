﻿/*
Huxley - a JSON proxy for the UK National Rail Live Departure Board SOAP API
Copyright (C) 2015 James Singleton
 * http://huxley.unop.uk
 * https://github.com/jpsingleton/Huxley

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Web.Http;
using Formo;
using Huxley.ldbServiceReference;

namespace Huxley.Controllers {
    public class BaseController : ApiController {
        protected static AccessToken MakeAccessToken(Guid accessToken) {
            dynamic config = new Configuration();
            var darwinAccessToken = config.DarwinAccessToken<Guid>(Guid.Empty);
            var clientAccessToken = config.ClientAccessToken<Guid>(Guid.Empty);
            if (clientAccessToken == accessToken) {
                accessToken = darwinAccessToken;
            }
            var token = new AccessToken { TokenValue = accessToken.ToString() };
            return token;
        }

        protected static string MakeCrsCode(string query) {
            // Process CRS codes if query is present
            if (!string.IsNullOrWhiteSpace(query) &&
                // If query is not in the list of CRS codes
                !WebApiApplication.CrsCodes.Any(c =>
                    c.CrsCode.Equals(query, StringComparison.InvariantCultureIgnoreCase))) {
                // And query matches a single station name
                var results = WebApiApplication.CrsCodes.Where(c =>
                    c.StationName.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                if (results.Count == 1) {
                    // Return the only possible CRS code
                    return results[0].CrsCode;
                }
                // If more than one match then return one if it matches exactly
                if (results.Count > 1) {
                    var bestMatch = results.FirstOrDefault(r =>
                        r.StationName.Equals(query, StringComparison.InvariantCultureIgnoreCase));
                    if (null != bestMatch) {
                        return bestMatch.CrsCode;
                    }
                }
            }
            // Otherwise return the query as is
            return query;
        }
    }
}
