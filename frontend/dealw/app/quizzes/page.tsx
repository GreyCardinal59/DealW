"use client";

import { Button, Table, Input, Typography, Space } from "antd";
import Modal from "antd/es/modal/Modal";
import { Quizzes } from "../components/Quizzes";
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
    const [mode, setMode] = useState(Mode.Create);
    const [searchText, setSearchText] = useState("");

    useEffect(() => {
        const getQuizzes = async () => {
            const quizzes = await getAllQuizzes();
            setLoading(false);
            setQuizzes(quizzes);
        };

        getQuizzes();
    }, []);

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

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchText(e.target.value);
    };

    const filteredQuizzes = quizzes.filter(quiz => 
        quiz.title.toLowerCase().includes(searchText.toLowerCase())
    );

    const columns = [
        {
            title: 'Title',
            dataIndex: 'title',
            key: 'title',
            render: (text: string, record: Quiz) => (
                <a onClick={() => handleRowClick(record)}>{text}</a>
            ),
        },
        {
            title: 'Action',
            key: 'action',
            render: (_: any, record: Quiz) => (
                <Space size="middle">
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
        </div>
    );
}