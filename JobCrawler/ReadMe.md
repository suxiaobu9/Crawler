# ?

- dotnet ef dbcontext scaffold "server=localhost,1433;database=JobCrawler;user=OneOFour;password=123456;" "Microsoft.EntityFrameworkCore.SqlServer" -o .\Model\Entity\ -c CrawlerContext -f

- docker build -f .\JobCrawlerDockerfile -t jobcrawler .

- docker run -d -p 5011:80 -e "ASPNETCORE_ENVIRONMENT=Production" --name JobCrawler {image id}