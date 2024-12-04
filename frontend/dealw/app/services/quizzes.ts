export interface QuizRequest {
    title: string;
}

export const getAllQuizzes = async () => {
    const response = await fetch("http://localhost:5059/Quizzes");

    return response.json();
};

export const createQuiz = async (quizRequest: QuizRequest) => {
    await fetch("http://localhost:5059/Quizzes", {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        body: JSON.stringify(quizRequest),
    });
};

export const updateQuiz = async (id: string, quizRequest: QuizRequest) => {
    await fetch(`http://localhost:5059/Quizzes/${id}`, {
        method: "PUT",
        headers: {
            "content-type": "application/json",
        },
        body: JSON.stringify(quizRequest),
    });
};

export const deleteQuiz = async (id: string) => {
    await fetch(`http://localhost:5059/Quizzes/${id}`, {
        method: "DELETE",
    });
};