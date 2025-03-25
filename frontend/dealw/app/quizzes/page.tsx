"use client";

import { Button, Table, Input, Typography, Space, Modal, Radio } from "antd";
import { useEffect, useState } from "react";
import { QuizRequest, createQuiz, deleteQuiz, getAllQuizzes, updateQuiz } from "../services/quizzes";
import { CreateUpdateQuiz, Mode } from "../components/CreateUpdateQuiz";

const { Title } = Typography;

export default function QuizzesPage() {
    const defaultValues = {
        title: "",
    } as Quiz;

    const [values, setValues] = useState<Quiz>(defaultValues);
    const [quizzes, setQuizzes] = useState<Quiz[]>([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isSolveModalOpen, setIsSolveModalOpen] = useState(false);
    const [mode, setMode] = useState(Mode.Create);
    const [searchText, setSearchText] = useState("");

    const [quizQuestions, setQuizQuestions] = useState<{ question: string; options: string[]; id: number }[]>([]);
    const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
    const [selectedAnswers, setSelectedAnswers] = useState<Record<number, string>>({});

    useEffect(() => {
        const getQuizzes = async () => {
            // const quizzes = await getAllQuizzes();

            // Хардкод
            const questions = [
                { id: 1, question: "Какого цвета небо?", options: ["Синий", "Зеленый", "Красный"] },
                { id: 2, question: "Сколько ног у паука?", options: ["6", "8", "10"] },
            ];

            const questions2 = [
                { id: 1, question: "Какого типа данных нет в Python?", options: ["Словарь (dict)", "Кортеж (tuple)", "double"] },
                { id: 2, question: "Компилируемый ли язык Python?", options: ["Да", "Нет"] },
            ];

            const questions3 = [
            {
            id: 1,
            question: "Какой метод используется для добавления элемента в список в Python?",
            options: ["append()", "add()", "insert()"],
            answer_id: 0
            },
            {
            id: 2,
            question: "Какой результат будет у выражения 2 ** 3 в Python?",
            options: ["6", "8", "9"],
            answer_id: 1
            },
            {
            id: 3,
            question: "Как называется неизменяемый тип коллекции в Python?",
            options: ["List", "Tuple", "Set"],
            answer_id: 1
            },
            {
            id: 4,
            question: "Что вернет функция len(['a', 'b', 'c'])?",
            options: ["2", "3", "4"],
            answer_id: 1
            },
            {
            id: 5,
            question: "Какой модуль используется для работы с случайными числами в Python?",
            options: ["random", "math", "numbers"],
            answer_id: 0
            },
            {
            id: 6,
            question: "Какая функция используется для вывода текста в консоль?",
            options: ["print()", "echo()", "write()"],
            answer_id: 0
            },
            {
            id: 7,
            question: "Какое ключевое слово используется для определения функции?",
            options: ["func", "def", "lambda"],
            answer_id: 1
            },
            {
            id: 8,
            question: "Какой тип данных возвращает выражение 5 / 2?",
            options: ["int", "float", "str"],
            answer_id: 1
            },
            {
            id: 9,
            question: "Что делает метод .split() у строки?",
            options: ["Соединяет строки", "Разбивает строку на части", "Удаляет пробелы"],
            answer_id: 1
            },
            {
            id: 10,
            question: "Какой из перечисленных операторов используется для сравнения?",
            options: ["=", "==", "!="],
            answer_id: 1
            },
            {
            id: 11,
            question: "Какой результат у выражения bool(0)?",
            options: ["True", "False", "Error"],
            answer_id: 1
            },
            {
            id: 12,
            question: "Какой метод удаляет последний элемент из списка?",
            options: ["remove()", "pop()", "delete()"],
            answer_id: 1
            },
            {
            id: 13,
            question: "Как в Python обозначается комментарий в одну строку?",
            options: ["//", "#", "/*"],
            answer_id: 1
            },
            {
            id: 14,
            question: "Какой результат у выражения '5' + '3'?",
            options: ["53", "8", "Error"],
            answer_id: 0
            },
            {
            id: 15,
            question: "Как называется тип данных для хранения ключ-значение?",
            options: ["List", "Tuple", "Dict"],
            answer_id: 2
            },
            {
            id: 16,
            question: "Какое ключевое слово используется для цикла с определенным количеством повторений?",
            options: ["while", "for", "loop"],
            answer_id: 1
            },
            {
            id: 17,
            question: "Какой из этих методов проверяет, есть ли ключ в словаре?",
            options: [".keys()", "in", ".contains()"],
            answer_id: 1
            },
            {
            id: 18,
            question: "Как называется ошибка деления на 0?",
            options: ["ValueError", "ZeroDivisionError", "NameError"],
            answer_id: 1
            },
            {
            id: 19,
            question: "Что делает конструкция try-except?",
            options: ["Создает функцию", "Обрабатывает ошибки", "Завершает программу"],
            answer_id: 1
            },
            {
            id: 20,
            question: "Какой модуль используется для работы с датами в Python?",
            options: ["datetime", "time", "date"],
            answer_id: 0
            },
            {
            id: 21,
            question: "Какой метод используется для сортировки списка?",
            options: ["sort()", "sorted()", "order()"],
            answer_id: 0
            },
            {
            id: 22,
            question: "Какой символ используется для логического И в Python?",
            options: ["&", "&&", "and"],
            answer_id: 2
            },
            {
            id: 23,
            question: "Как обозначаются неиспользуемые параметры в функции?",
            options: ["_", "x", "None"],
            answer_id: 0
            },
            {
            id: 24,
            question: "Что вернет выражение type(42)?",
            options: ["int", "float", "str"],
            answer_id: 0
            },
            {
            id: 25,
            question: "Какое ключевое слово используется для создания класса?",
            options: ["class", "def", "struct"],
            answer_id: 0
            }
            ];

            const quizzes = [
                {id: 1, title: "test1", questions: questions},
                {id: 2, title: "test2", questions: questions2},
                {id: 3, title: "test3", questions: questions3},
                {id: 4, title: "test4", questions: questions2},
                {id: 5, title: "test5", questions: questions2},
            ]

            setLoading(false);
            setQuizzes(quizzes);
        };

        getQuizzes();
    }, []);

    const the_answer = 0;

    const handleCreateQuiz = async (request: QuizRequest) => {
        await createQuiz(request);
        closeModal();
        await loadQuizzes();
    };

    const handleUpdateQuiz = async (id: string, request: QuizRequest) => {
        await updateQuiz(id, request);
        closeModal();
        await loadQuizzes();
    };

    const handleDeleteQuiz = async (id: string) => {
        await deleteQuiz(id);
        await loadQuizzes();
    };

    const loadQuizzes = async () => {
        const quizzes = await getAllQuizzes();
        setQuizzes(quizzes);
    };

    const openModal = () => {
        setMode(Mode.Create);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        loadQuizzes();
        setValues(defaultValues);
        setIsModalOpen(false);
    };

    const openEditModal = (quiz: Quiz) => {
        setMode(Mode.Edit);
        setValues(quiz);
        setIsModalOpen(true);
    };

    const handleRowClick = (quiz: Quiz) => {
        Modal.info({
            title: quiz.title,
            content: (
                <div>
                    <p>Информация о квизе: {quiz.title}</p>
                </div>
            ),
            onOk() {},
            onCancel() {},
        });
    };

    const openSolveModal = (quiz: Quiz) => {

        const questions = quiz.questions;

        setQuizQuestions(questions);
        setCurrentQuestionIndex(0);
        setSelectedAnswers({});
        setIsSolveModalOpen(true);
    };

    const closeSolveModal = () => {
        setQuizQuestions([]);
        setCurrentQuestionIndex(0);
        setSelectedAnswers({});
        setIsSolveModalOpen(false);
    };

    const handleOptionChange = (e: any) => {
        const answer = e.target.value;
        const questionId = quizQuestions[currentQuestionIndex].id;
        setSelectedAnswers((prevAnswers) => ({ ...prevAnswers, [questionId]: answer }));
    };

    const handleAnswer = (quiz: Quiz) => {
        
    };

    const handleNext = async () => {
        if (currentQuestionIndex < quizQuestions.length - 1) {
            setCurrentQuestionIndex(currentQuestionIndex + 1);
        } else {
            await submitAnswers();
            closeSolveModal();
        }
    };

    const submitAnswers = async () => {
        const data = {
            quizId: quizzes[0]?.id,
            answers: selectedAnswers,
        };

        // TODO: реально отправлять данные на сервер.
        console.log("Ответы отправлены на сервер:", data);
    };

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchText(e.target.value);
    };

    const filteredQuizzes = quizzes.filter((quiz) =>
        quiz.title.toLowerCase().includes(searchText.toLowerCase())
    );

    const columns = [
        {
            title: "Title",
            dataIndex: "title",
            key: "title",
            render: (text: string, record: Quiz) => (
                <a onClick={() => openSolveModal(record)}>{text}</a>
            ),
        },
        {
            title: "Action",
            key: "action",
            render: (_: any, record: Quiz) => (
                <Space size="middle">
                    <Button onClick={() => openSolveModal(record)}>Решить</Button>
                    <Button onClick={() => openEditModal(record)}>Редактировать</Button>
                    <Button onClick={() => handleDeleteQuiz(record.id)}>Удалить</Button>
                </Space>
            ),
        },
    ];

    return (
        <div>
            <Input
                placeholder="Поиск по названию квиза"
                value={searchText}
                onChange={handleSearchChange}
                style={{ marginBottom: 20 }}
            />
            <Button
                type="primary"
                style={{ marginBottom: "20px" }}
                size="large"
                onClick={openModal}
            >
                Добавить квиз
            </Button>

            <CreateUpdateQuiz
                mode={mode}
                values={values}
                isModalOpen={isModalOpen}
                handleCreate={handleCreateQuiz}
                handleUpdate={handleUpdateQuiz}
                handleCancel={closeModal}
            />

            {loading ? (
                <Title>Loading...</Title>
            ) : (
                <Table
                    dataSource={filteredQuizzes}
                    columns={columns}
                    rowKey="id"
                />
            )}

            {/* TODO: Создать отдельную модель для этого модального окна? */}
            <Modal
                title={`Вопрос ${currentQuestionIndex + 1}/${quizQuestions.length} 30 сек.`}
                open={isSolveModalOpen}
                onCancel={closeSolveModal}
                footer={[
                    <Button key="next" type="primary" onClick={handleAnswer}>
                        Ответить
                    </Button>,
                    <Button key="next" type="primary" onClick={handleNext}>
                        {currentQuestionIndex < quizQuestions.length - 1 ? "Далее" : "Завершить"}
                    </Button>,
                ]}
            >
                {quizQuestions.length > 0 && (
                    <div>
                        <p>{quizQuestions[currentQuestionIndex].question}</p>
                        <Radio.Group
                            onChange={handleOptionChange}
                            value={selectedAnswers[quizQuestions[currentQuestionIndex].id] || null}
                        >
                            {quizQuestions[currentQuestionIndex].options.map((option, index) => (
                                <Radio key={index} value={option}>
                                    {option}
                                </Radio>
                            ))}
                        </Radio.Group>
                    </div>
                )}
            </Modal>
        </div>
    );
}
