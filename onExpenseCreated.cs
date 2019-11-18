using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace expense_fn
{
    public static class onExpenseCreated
    {
        [FunctionName("onExpenseCreated")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post",
            Route = null)] HttpRequest req,
            [Queue("expenses")] IAsyncCollector<Expense> expenseQueue,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function. Expense created");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Expense expense = JsonConvert.DeserializeObject<Expense>(requestBody);
            // Send to storage queue
            await expenseQueue.AddAsync(expense);

            log.LogInformation($"Expense {expense.ExpenseId} received for {expense.Description}");

            return new OkObjectResult($"Expense has been created in the queue");
        }
    }
}
