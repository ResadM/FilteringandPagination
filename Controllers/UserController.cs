using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;



namespace FilteringandPagination.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly List<Users> users = new List<Users>{
            new Users { user_id=1,user_name="Simon",user_age=25 },
            new Users { user_id=2,user_name="Jones",user_age=30 },
            new Users { user_id=3,user_name="Adrian",user_age=21 },
            new Users { user_id=4,user_name="Grace",user_age=32 },
            new Users { user_id=5,user_name="Chloe",user_age=36 },
            new Users { user_id=6,user_name="Lily",user_age=24 },
            new Users { user_id=7,user_name="Joan",user_age=38 },
            new Users { user_id=8,user_name="Jake",user_age=20 },
            new Users { user_id=9,user_name="Boris",user_age=26 },
            new Users { user_id=10,user_name="Caroline",user_age=24 },
            new Users { user_id=11,user_name="Connor",user_age=27 },
            new Users { user_id=12,user_name="Vanessa",user_age=25 },
            new Users { user_id=13,user_name="Dorothy",user_age=35 },
            new Users { user_id=14,user_name="Deirdre",user_age=36 },
            new Users { user_id=15,user_name="Melanie",user_age=25 },

        };

        [HttpGet(Name = "getUsers")]
        public IEnumerable<Users> Get([FromQuery] SearchParams searchParam)
        {

            List<ColumnFilter> columnFilters = new List<ColumnFilter>();
            if (!String.IsNullOrEmpty(searchParam.ColumnFilters))
            {
                try
                {
                    columnFilters.AddRange(JsonSerializer.Deserialize<List<ColumnFilter>>(searchParam.ColumnFilters));
                }
                catch (Exception)
                {
                    columnFilters = new List<ColumnFilter>();
                }
            }

            List<ColumnSorting> columnSorting = new List<ColumnSorting>();
            if (!String.IsNullOrEmpty(searchParam.OrderBy))
            {
                try
                {
                    columnSorting.AddRange(JsonSerializer.Deserialize<List<ColumnSorting>>(searchParam.OrderBy));
                }
                catch (Exception)
                {
                    columnSorting = new List<ColumnSorting>();
                }
            }

            Expression<Func<Users, bool>> filters = null;
            //First, we are checking our SearchTerm. If it contains information we are creating a filter.
            var searchTerm = "";
            if (!string.IsNullOrEmpty(searchParam.SearchTerm))
            {
                searchTerm = searchParam.SearchTerm.Trim().ToLower();
                filters = x => x.user_name.ToLower().Contains(searchTerm);
            }
            // Then we are overwriting a filter if columnFilters has data.
            if (columnFilters.Count > 0)
            {
                var customFilter = CustomExpressionFilter<Users>.CustomFilter(columnFilters, "users");
                filters = customFilter;
            }


            var query = users.AsQueryable().CustomQuery(filters);
            var count=query.Count();
            var filteredData = query.CustomPagination(searchParam.PageNumber,searchParam.PageSize).ToList();

            var pagedList = new PagedList<Users>(filteredData, count, searchParam.PageNumber, searchParam.PageSize);

            if (pagedList != null)
            {
                Response.AddPaginationHeader(pagedList.MetaData);
            }

            return pagedList;
        }
    }
}
