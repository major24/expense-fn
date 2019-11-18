using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace expense_fn
{
    public static class onExpenseQueueCreated
    {
        [FunctionName("onExpenseQueueCreated")]
        public static void Run(
          [QueueTrigger("expenses", Connection = "AzureWebJobsStorage")] Expense expense,
          [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob,
          ILogger log)
        {
          log.LogInformation($"Queue trigger func. Expense Queue has been created {expense}");

          outputBlob.WriteLine($"ExpenseId: {expense.ExpenseId}");
          outputBlob.WriteLine($"Description: {expense.Description}");
          outputBlob.WriteLine($"ExpenseSubmittedDate: {DateTime.UtcNow}");
          var md5 = System.Security.Cryptography.MD5.Create();
          var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(expense.Description + "secret"));
          outputBlob.WriteLine($"SecretCode: {System.BitConverter.ToString(hash).Replace("-", "")}");
        }
    }
}
