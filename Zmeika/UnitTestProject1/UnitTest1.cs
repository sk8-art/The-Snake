using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using Zmeika;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private MainWindow mainWindow;

        [TestInitialize]
        public void Setup()
        {
            mainWindow = new MainWindow();
            mainWindow.InitialGame();
        }

        [TestMethod]
        public void TestEatingFoodIncreasesScore()
        {
            // Размещаем еду прямо перед головой змейки
            mainWindow.PlaceFood();
            mainWindow._foodPosition = new Point(5, 5); // Делаем так, чтобы еда была на позиции головы змейки
            mainWindow.EatFood(); // Симулируем поедание еды

            Assert.AreEqual(1, mainWindow._score); // Проверяем, что счёт увеличился
            Assert.AreEqual(2, mainWindow._snake.Count); // Проверяем, что длина змейки увеличилась
        }

    }
}
