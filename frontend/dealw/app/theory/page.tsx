import { Button, Table, Input, Typography, Space, Modal, Radio } from "antd";

export default function Theory() {
  const topics = [
    "Что такое Python и его основные особенности",
    "Установка Python и настройка окружения",
    "Основы синтаксиса Python",
    "Переменные и типы данных в Python",
    "Условные операторы: if, elif, else",
    "Циклы: for и while",
    "Функции: объявление, параметры и возвращаемые значения",
    "Списки, кортежи и словари",
    "Модули и библиотеки Python",
    "Обработка исключений в Python",
    "Работа с файлами: чтение и запись",
    "Классы и основы ООП в Python",
    "Основы работы с библиотекой NumPy",
    "Построение графиков с использованием Matplotlib",
    "Введение в асинхронное программирование",
  ];

  return (
    <div style={styles.container}>
      <h1 style={styles.title}>Теория Python</h1>
      <ul style={styles.list}>
        {topics.map((topic, index) => (
          <Button key={index} style={styles.listItem}>
            {topic}
          </Button>
        ))}
      </ul>
    </div>
  );
}

const styles = {
  container: {
    padding: "20px",
    fontFamily: "Arial, sans-serif",
  },
  title: {
    fontSize: "2.5rem",
    marginBottom: "20px",
    color: "#333",
  },
  list: {
    listStyleType: "disc",
    paddingLeft: "20px",
  },
  listItem: {
    fontSize: "1.2rem",
    margin: "10px 0",
  },
};
