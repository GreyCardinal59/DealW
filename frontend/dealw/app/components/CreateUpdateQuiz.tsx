import Modal from "antd/es/modal/Modal";
import { QuizRequest } from "../services/quizzes";
import { Input } from "antd";
import { useEffect, useState } from "react";
import TextArea from "antd/es/input/TextArea";

interface Props {
    mode: Mode;
    values: Quiz;
    isModalOpen: boolean;
    handleCancel: () => void;
    handleCreate: (request: QuizRequest) => void;
    handleUpdate: (id: string, request: QuizRequest) => void; 
};

export enum Mode {
    Create,
    Edit,
};

export const CreateUpdateQuiz = ({
    mode,
    values,
    isModalOpen,
    handleCancel,
    handleCreate,
    handleUpdate,
}: Props) => {
    const [title, setTitle] = useState<string>("");

    useEffect(() => {
        setTitle(values.title);
    }, [values]);

    const handleOnOk = async () => {
        const quizRequest = {title};

        mode == Mode.Create 
        ? handleCreate(quizRequest) 
        : handleUpdate(values.id, quizRequest);
    };

    return (
        <Modal 
            title={
                mode === Mode.Create ? "Добавить квиз" : "Редактировать"
            }
            open={isModalOpen}
            onOK={handleOnOk}
            onCancel={handleCancel}
            cancelText={"Отмена"}
        >
            <div className="quiz__modal">
                <Input
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder="Название"
                />
            </div>
        </Modal>
    );
};