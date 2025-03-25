interface Question {
    id: string;
    question: string;
    options: string[];
    correct_id: int;
}

interface Quiz {
    id: string;
    title: string;
    questions: Question[];
}