export interface QuizRequest {
    title: string;
}

export const getAllQuizzes = async () => {
    try {
        const response = await fetch("http://localhost:3000/quizzes");
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Failed to fetch quizzes:", error);
        throw error;
    }
};

export const createQuiz = async (quizRequest: QuizRequest) => {
    await fetch("http://localhost:3000/quizzes", {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        body: JSON.stringify(quizRequest),
    });
};

export const updateQuiz = async (id: string, quizRequest: QuizRequest) => {
    await fetch(`http://localhost:3000/quizzes/${id}`, {
        method: "PUT",
        headers: {
            "content-type": "application/json",
        },
        body: JSON.stringify(quizRequest),
    });
};

export const deleteQuiz = async (id: string) => {
    await fetch(`http://localhost:3000/quizzes/${id}`, {
        method: "DELETE",
    });
};

