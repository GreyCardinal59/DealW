using System.Runtime.CompilerServices;
using DealW.Application.Services;
using DealW.Contracts;
using DealW.Domain.Abstractions;
using DealW.Domain.Enums;
using DealW.Domain.Models;
using DealW.Infrastructure;
using DealW.Infrastructure.Authentication;
using DealW.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizationOptions = DealW.Persistence.AuthorizationOptions;

namespace DealW.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizzesController(IDuelsService quizzesService, IQuestionsRepository questionsRepository) : ControllerBase
{
    // [HttpGet]
    // [RequirePermissions(Permission.Read)]
    // public async Task<ActionResult<List<QuizzesResponse>>> GetQuizzes()
    // {
    //     
    //     var quizzes = await quizzesService.GetAllQuizzes();
    //
    //     var response = quizzes.Select(q => new QuizzesResponse(q.Id, q.Title, q.Difficulty));
    //     
    //     return Ok(response);
    // }

    // [HttpPost]
    // public async Task<ActionResult<int>> CreateQuiz([FromBody] QuizzesRequest request)
    // {
    //     var (quiz, error) = Duel.Create(
    //         0,
    //         request.title,
    //         request.difficulty);
    //     if (!string.IsNullOrEmpty(error))
    //     {
    //         return BadRequest(error);
    //     }
    //     
    //     var quizId = await quizzesService.CreateQuiz(quiz);
    //     
    //     return Ok(quizId);
    // }

    // [HttpPut("{id:int}")]
    // public async Task<ActionResult<int>> UpdateQuizzes(int id, [FromBody] QuizzesRequest request)
    // {
    //     var quizId = await quizzesService.UpdateQuiz(id, request.title, request.difficulty);
    //     
    //     return Ok(quizId);
    // }

    // [HttpDelete("{id:int}")]
    // public async Task<ActionResult<int>> DeleteQuiz(int id)
    // {
    //     return Ok(await quizzesService.DeleteDuel(id));
    // }
    //
    // [HttpGet("{id:int}")]
    // [RequirePermissions(Permission.Read, Permission.Create, Permission.Update, Permission.Delete)]
    // public async Task<ActionResult<List<QuestionResponse>>> GetQuestionsByQuizId(int id)
    // {
    //     var questions = await questionsRepository.GetByQuizId(id);
    //
    //     var response = questions.Select(q => new QuestionResponse(q.Id, q.QuizId, q.Text, q.CorrectAnswerId));
    //     
    //     return Ok(response);
    // }
}