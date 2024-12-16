using DealW.Application.Services;
using DealW.Contracts;
using DealW.Domain.Abstractions;
using DealW.Domain.Models;
using DealW.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DealW.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizzesController(IQuizzesService quizzesService, IQuestionsRepository questionsRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<QuizzesResponse>>> GetQuizzes()
    {
        
        var quizzes = await quizzesService.GetAllQuizzes();

        var response = quizzes.Select(q => new QuizzesResponse(q.Id, q.Title, q.Difficulty));
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateQuiz([FromBody] QuizzesRequest request)
    {
        var (quiz, error) = Quiz.Create(
            0,
            request.title,
            request.difficulty);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        
        var quizId = await quizzesService.CreateQuiz(quiz);
        
        return Ok(quizId);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<int>> UpdateQuizzes(int id, [FromBody] QuizzesRequest request)
    {
        var quizId = await quizzesService.UpdateQuiz(id, request.title, request.difficulty);
        
        return Ok(quizId);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<int>> DeleteQuiz(int id)
    {
        return Ok(await quizzesService.DeleteQuiz(id));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<List<QuestionResponse>>> GetQuestionsByQuizId(int id)
    {
        var questions = await questionsRepository.GetByQuizId(id);

        var response = questions.Select(q => new QuestionResponse(q.Id, q.QuizId, q.Text, q.CorrectAnswerId));
        
        return Ok(response);
    }
}