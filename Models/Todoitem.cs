using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListApp.Models
{
    public class Todoitem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public bool Done { get; set; }

        public bool IsSelected { get; set; }

        public DateTime Date { get; set; }

        public string Priority { get; set; }

        public Byte[] Attachment { get; set; }

        public bool HasAttachment { get; set; }

        public bool IsPinned { get; set; }

        public Color PriorityColor
        {
            get
            {
                switch (Priority)
                {
                    case "Critical":
                        return Color.FromArgb("#555555");
                    case "High":
                        return Colors.Red;
                    case "Medium":
                        return Colors.Yellow;
                    case "Low":
                        return Colors.LightGreen;
                    default:
                        return Colors.White;
                }
            }
        }
    }
}