using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.App_Data.Entities
{
  public class ApiUser
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Secret { get; set; }
    public string AppId { get; set; }
  }
}
