using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using System;
using System.Linq;

namespace MvcMovie.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new MvcMovieContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<MvcMovieContext>>()))
        {
            // Look for any movies.
            if (context.Movie.Any())
            {
                return;   // DB has been seeded
            }
            context.Movie.AddRange(
                new Movie
                {
                    Title = "Ghostbusters ",
                    ReleaseDate = DateTime.Parse("1984-3-13"),
                    Genre = "Comedy",
                    Rating = "PG",
                    Price = 8.99M
                },
                new Movie
                {
                    Title = "Ghostbusters 2",
                    ReleaseDate = DateTime.Parse("1986-2-23"),
                    Genre = "Comedy",
                    Rating = "PG",
                    Price = 9.99M
                },
                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.Parse("1959-4-15"),
                    Genre = "Western",
                    Rating = "NR",
                    Price = 3.99M
                },
                new Movie
                {
                    Title = "The Angry Birds Movie",
                    ReleaseDate = DateTime.Parse("2016-5-20"),
                    Genre = "Animation",
                    Rating = "PG",
                    Price = 5.99M
                },
                new Movie
                {
                    Title = "Transformers 3",
                    ReleaseDate = DateTime.Parse("2011-6-29"),
                    Genre = "Action",
                    Rating = "PG-13",
                    Price = 8.49M
                },
                new Movie
                {
                    Title = "Finding Nemo",
                    ReleaseDate = DateTime.Parse("2003-5-30"),
                    Genre = "Animation",
                    Rating = "G",
                    Price = 6.49M
                },
                new Movie
                {
                    Title = "Over the Hedge",
                    ReleaseDate = DateTime.Parse("2006-5-19"),
                    Genre = "Animation",
                    Rating = "PG",
                    Price = 5.49M
                },
                new Movie
                {
                    Title = "Hunt for the Wilderpeople",
                    ReleaseDate = DateTime.Parse("2016-6-24"),
                    Genre = "Adventure",
                    Rating = "PG-13",
                    Price = 7.49M
                }
            );
            context.SaveChanges();
        }
    }
}
