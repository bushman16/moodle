using System;

namespace db_interact
{
    public interface db_interact
    {
        /*
         интерфейс
         */
        public void connect();
    }
    public class sql_server : db_interact
    {
        public sql_server() { }
        public void connect() { }
    }
}
