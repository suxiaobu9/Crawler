# ?

- dotnet ef dbcontext scaffold "server=localhost,1433;database=JobCrawler;user=OneOFour;password=123456;" "Microsoft.EntityFrameworkCore.SqlServer" -o .\JobCrawler\Entity\ -c JobCrawlerContext -f

- dotnet ef dbcontext scaffold "server=localhost,1433;database=StockCrawler;user=Stock;password=123456;" "Microsoft.EntityFrameworkCore.SqlServer" -o .\StockCrawler\Entity\ -c StockCrawlerContext -f

- docker build -f .\JobCrawlerDockerfile -t jobcrawler .

- docker run -d -p 5011:80 -e "ASPNETCORE_ENVIRONMENT=Production" --name JobCrawler {image id}