using System;

namespace file_opener
{
    public interface file_opener
    {
        /*
         Интерфей.
        сюда пишим функции которые будут у всех открывателей файлов
         */
        public void open();
    }
    public class cvs_opener : file_opener
    {
        /*
         пример открывателя файлов
        здесь реализация
         */
        public cvs_opener() { }
        public void open() { 
        /*
         реализация open()
         */
        }
    }
}
