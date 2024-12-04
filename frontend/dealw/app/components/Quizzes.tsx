import Card from "antd/es/card/Card";
import { CardTitle } from "./Cardtitle";
import Button from "antd/es/button/button";

interface Props {
    quizzes: Quiz[];
    handleDelete: (id: string) => void;
    handleOpen: (quiz: Quiz) => void;
}

export const Quizzes = ({ quizzes, handleDelete, handleOpen }: Props) => {
    return (
        <div className="cards">
            {quizzes.map((quiz : Quiz) => (
                <Card 
                    key={quiz.id} 
                    title={<CardTitle title={quiz.title} />} 
                    bordered={false}
                >
                    <p>{quiz.title}</p>
                    <div className="card_buttons">
                        <Button 
                            onClick={() => handleOpen(quiz)}
                            style ={{ flex: 1 }}
                        >
                            Редактировать
                        </Button>
                        <Button
                            onClick={() => handleDelete(quiz.id)}
                            danger
                            style= {{ flex: 1 }}
                        >
                            Удалить
                        </Button>
                    </div>
                </Card>
            ))}
        </div>
    );
};