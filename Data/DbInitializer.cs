using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using gfps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gfps.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Apply migrations at startup (creates db if it doesn't exist and applies pending migrations)
            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            string[] roleNames = { "SuperAdmin", "Admin", "ContentManager", "AdmissionsStaff" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Seed Admin Users
            await SeedUserAsync(userManager, "superadmin@gfps.edu", "Super Admin", "SuperAdmin", "SuperAdmin@123");
            await SeedUserAsync(userManager, "admin@gfps.edu", "Site Admin", "Admin", "Admin@123");
            await SeedUserAsync(userManager, "content@gfps.edu", "Content Manager", "ContentManager", "Content@123");
            await SeedUserAsync(userManager, "admissions@gfps.edu", "Admissions Staff", "AdmissionsStaff", "Admissions@123");

            // 3. Seed SEO Metadata
            if (!context.SeoMetadata.Any())
            {
                var seoList = new List<SeoMetadata>
                {
                    new SeoMetadata
                    {
                        PageName = "Home",
                        Title = "Green Field Public School | Empowering Future Leaders",
                        MetaDescription = "A premium educational ecosystem designed to foster innovation, critical thinking, and holistic development in state-of-the-art facilities.",
                        OpenGraphTitle = "Green Field Public School",
                        OpenGraphDescription = "Empowering Future Leaders Through Excellence in Education.",
                        OpenGraphImage = "/images/img_31.jpg",
                        CanonicalUrl = "/"
                    },
                    new SeoMetadata
                    {
                        PageName = "About",
                        Title = "About Us | Green Field Public School",
                        MetaDescription = "A legacy of nurturing excellence since 1995. Read about our core philosophy, vision, mission, and school directors.",
                        OpenGraphTitle = "About Green Field Public School",
                        OpenGraphDescription = "A Legacy of Nurturing Excellence since 1995.",
                        OpenGraphImage = "/images/img_1.jpg",
                        CanonicalUrl = "/Home/About"
                    },
                    new SeoMetadata
                    {
                        PageName = "Academics",
                        Title = "Academic Programs | Green Field Public School",
                        MetaDescription = "Explore our Science, Commerce, and Humanities curriculum breakdown. CBSE curriculum combined with global pedagogical standards.",
                        OpenGraphTitle = "Academics - Green Field Public School",
                        OpenGraphDescription = "CBSE board standards combined with global teaching methodologies.",
                        OpenGraphImage = "/images/img_31.jpg",
                        CanonicalUrl = "/Home/Academics"
                    },
                    new SeoMetadata
                    {
                        PageName = "Faculty",
                        Title = "Faculty Members | Green Field Public School",
                        MetaDescription = "Meet the distinguished educators, scholars, and experts shaping the future of students at Green Field Public School.",
                        OpenGraphTitle = "Faculty - Green Field Public School",
                        OpenGraphDescription = "Meet our expert teaching faculty.",
                        OpenGraphImage = "/images/img_32.jpg",
                        CanonicalUrl = "/Home/Faculty"
                    },
                    new SeoMetadata
                    {
                        PageName = "EventsNews",
                        Title = "Events & News | Green Field Public School",
                        MetaDescription = "Stay updated with recent news, featured stories, and upcoming calendar events at Green Field Public School.",
                        OpenGraphTitle = "Events & News - Green Field Public School",
                        OpenGraphDescription = "Never miss an update from our campus.",
                        OpenGraphImage = "/images/img_15.jpg",
                        CanonicalUrl = "/Home/EventsNews"
                    },
                    new SeoMetadata
                    {
                        PageName = "Gallery",
                        Title = "Campus Gallery | Green Field Public School",
                        MetaDescription = "View pictures and videos of our advanced science labs, athletics meet, arts festival, robotics workshops, and graduation ceremonies.",
                        OpenGraphTitle = "Gallery - Green Field Public School",
                        OpenGraphDescription = "Capturing excellence in action.",
                        OpenGraphImage = "/images/img_37.jpg",
                        CanonicalUrl = "/Home/Gallery"
                    },
                    new SeoMetadata
                    {
                        PageName = "Facilities",
                        Title = "Infrastructure & Facilities | Green Field Public School",
                        MetaDescription = "Tour our world-class infrastructure including smart classrooms, STEM labs, Olympic-size sports complex, and modern library.",
                        OpenGraphTitle = "Facilities - Green Field Public School",
                        OpenGraphDescription = "World-class ecosystem of educational excellence.",
                        OpenGraphImage = "/images/img_31.jpg",
                        CanonicalUrl = "/Home/Facilities"
                    },
                    new SeoMetadata
                    {
                        PageName = "Achievements",
                        Title = "Our Achievements | Green Field Public School",
                        MetaDescription = "Timeline of achievements, student hall of fame, institutional accolades, and athletic milestones.",
                        OpenGraphTitle = "Achievements - Green Field Public School",
                        OpenGraphDescription = "Our milestones of success.",
                        OpenGraphImage = "/images/img_11.jpg",
                        CanonicalUrl = "/Home/Achievements"
                    },
                    new SeoMetadata
                    {
                        PageName = "Contact",
                        Title = "Contact Us | Green Field Public School",
                        MetaDescription = "Get in touch with our admissions office, administration, or human resources. Locate our campus on the map.",
                        OpenGraphTitle = "Contact Us - Green Field Public School",
                        OpenGraphDescription = "Connect with us today.",
                        OpenGraphImage = "/images/img_24.jpg",
                        CanonicalUrl = "/Home/Contact"
                    },
                    new SeoMetadata
                    {
                        PageName = "Admissions",
                        Title = "Admissions & Entry | Green Field Public School",
                        MetaDescription = "Understand our step-by-step admissions journey, fee structures, downloadable prospectus, and FAQs.",
                        OpenGraphTitle = "Admissions - Green Field Public School",
                        OpenGraphDescription = "Join the legacy of academic excellence.",
                        OpenGraphImage = "/images/img_23.jpg",
                        CanonicalUrl = "/Admissions"
                    }
                };

                context.SeoMetadata.AddRange(seoList);
            }

            // 4. Seed Faculty Members
            if (!context.FacultyMembers.Any())
            {
                var faculty = new List<FacultyMember>
                {
                    new FacultyMember
                    {
                        Name = "Dr. Eleanor Vance",
                        Designation = "Head of Science Dept.",
                        Department = "Science",
                        Bio = "Ph.D. in Physics from Stanford with over 15 years of academic and research experience. Passionate about experiential science learning.",
                        ImagePath = "/images/img_32.jpg",
                        DisplayOrder = 1
                    },
                    new FacultyMember
                    {
                        Name = "Prof. Marcus Chen",
                        Designation = "Senior Mathematics Faculty",
                        Department = "Science",
                        Bio = "M.Sc. in Applied Mathematics. Specializes in advanced calculus and preparing students for competitive engineering entry tests.",
                        ImagePath = "/images/img_33.jpg",
                        DisplayOrder = 2
                    },
                    new FacultyMember
                    {
                        Name = "Sarah Jenkins",
                        Designation = "Literature & Humanities Dean",
                        Department = "Humanities",
                        Bio = "M.A. in English Literature from Oxford. Dedicated to cultivating analytical writing and a love of classics in young minds.",
                        ImagePath = "/images/img_34.jpg",
                        DisplayOrder = 3
                    },
                    new FacultyMember
                    {
                        Name = "David Russo",
                        Designation = "Director of Fine Arts",
                        Department = "Cultural",
                        Bio = "BFA from Yale School of Art. Active curator and educator with 20+ years of training students in sculpture, painting, and design.",
                        ImagePath = "/images/img_35.jpg",
                        DisplayOrder = 4
                    }
                };

                context.FacultyMembers.AddRange(faculty);
            }

            // 5. Seed Academic Programs (Curriculum)
            if (!context.AcademicPrograms.Any())
            {
                var programs = new List<AcademicProgram>
                {
                    new AcademicProgram
                    {
                        Title = "Science Stream",
                        Department = "Science",
                        Description = "Fostering analytical thinking and innovation through intensive theoretical study and hands-on laboratory work in Physics, Chemistry, Biology, and Computer Science.",
                        FullDescription = "<p>The Science Stream at Green Field Public School is designed to foster a spirit of inquiry and analytical skills. Through a balance of rigorous theoretical frameworks and intensive laboratory sessions, students explore advanced scientific paradigms.</p><ul><li>State-of-the-art physics, chemistry, and biology labs.</li><li>Integration of modern computer science concepts including Python programming and data structures.</li><li>Preparation for national level engineering and medical entrance tests.</li></ul>",
                        ClassOrGrade = "Grade XI & XII",
                        Subject = "Physics, Chemistry, Mathematics, Biology, Computer Science",
                        CurriculumJson = "[\"Physics (Mechanics, Electromagnetism, Quantum Theory)\", \"Chemistry (Organic, Inorganic, Physical)\", \"Biology (Genetics, Physiology, Ecology)\", \"Computer Science (Python, SQL, Data Structures)\"]",
                        Icon = "biotech",
                        FeaturedImage = "/images/img_37.jpg",
                        DisplayOrder = 1
                    },
                    new AcademicProgram
                    {
                        Title = "Commerce Stream",
                        Department = "Commerce",
                        Description = "Preparing future leaders of the business world with a strong foundation in Accountancy, Business Studies, Economics, and foundational Mathematics.",
                        FullDescription = "<p>The Commerce Stream equips students with critical business intelligence, analytical financial knowledge, and management methodologies. It prepares students for successful careers in entrepreneurship, finance, and administration.</p><ul><li>Focus on real-world case studies and market dynamics.</li><li>Comprehensive curriculum in financial bookkeeping, accountancy practices, and business ethics.</li><li>In-depth economics instruction covering macro and micro-level operations.</li></ul>",
                        ClassOrGrade = "Grade XI & XII",
                        Subject = "Accountancy, Business Studies, Economics, Mathematics",
                        CurriculumJson = "[\"Accountancy (Financial statements, Partnership accounts)\", \"Business Studies (Principles of Management, Marketing)\", \"Economics (Microeconomics, Macroeconomics, Statistics)\", \"Mathematics (Calculus, Probability, Linear Programming)\"]",
                        Icon = "query_stats",
                        FeaturedImage = "/images/img_31.jpg",
                        DisplayOrder = 2
                    },
                    new AcademicProgram
                    {
                        Title = "Humanities Stream",
                        Department = "Humanities",
                        Description = "Cultivating deep understanding of human society, culture, and history through specialized courses in History, Political Science, Psychology, and Sociology.",
                        FullDescription = "<p>The Humanities Stream offers an expansive liberal arts perspective that cultivates critical reading, sociological analysis, and empathy. Students engage with historical evolution, global political frameworks, and human psychology.</p><ul><li>Focus on analytical essay writing, seminars, and debates.</li><li>Field visits and projects addressing local community challenges.</li><li>Dynamic social psychology workshops.</li></ul>",
                        ClassOrGrade = "Grade XI & XII",
                        Subject = "History, Political Science, Psychology, Sociology",
                        CurriculumJson = "[\"History (World History, Modern Indian History)\", \"Political Science (Global politics, Constitutions)\", \"Psychology (Developmental, Cognitive, Social)\", \"Sociology (Social structures, Research methods)\"]",
                        Icon = "auto_stories",
                        FeaturedImage = "/images/img_34.jpg",
                        DisplayOrder = 3
                    }
                };

                context.AcademicPrograms.AddRange(programs);
            }

            // 6. Seed Events
            if (!context.Events.Any())
            {
                var events = new List<Event>
                {
                    new Event
                    {
                        Title = "Global Science Innovation Summit 2024",
                        EventDate = DateTime.Now.AddDays(15),
                        EventTime = "09:00 AM - 04:00 PM",
                        Location = "Main Auditorium & Science Wing",
                        ShortDescription = "A prestigious gathering showcasing student robotics designs, research posters, and hosting talks by industry specialists.",
                        FullDescription = "<p>We are hosting our annual Global Science Innovation Summit. This event brings together students from across the country to showcase their robotic designs, present research posters in physics and biochemistry, and participate in panel debates. Keynote speakers include researchers from premier institutes.</p>",
                        ImagePath = "/images/img_3.jpg",
                        GalleryImagesJson = "[\"/images/img_37.jpg\", \"/images/img_41.jpg\"]",
                        RegistrationLink = "/Home/RegisterEvent",
                        IsPublished = true
                    },
                    new Event
                    {
                        Title = "Annual Athletics Meet 2024",
                        EventDate = DateTime.Now.AddDays(30),
                        EventTime = "08:00 AM - 02:00 PM",
                        Location = "Olympic-Size Sports Complex",
                        ShortDescription = "Inter-house track and field events, football championships, and gymnast drills showing athletic prowess.",
                        FullDescription = "<p>Our annual sports festival will showcase track races, high jump, long jump, football finals, and rhythmic gymnastics. All parents are invited to cheer on our young sports champions.</p>",
                        ImagePath = "/images/img_4.jpg",
                        GalleryImagesJson = "[\"/images/img_38.jpg\"]",
                        RegistrationLink = "/Home/RegisterEvent",
                        IsPublished = true
                    },
                    new Event
                    {
                        Title = "Spring Arts Festival",
                        EventDate = DateTime.Now.AddDays(45),
                        EventTime = "10:30 AM - 06:00 PM",
                        Location = "Atrium Gallery",
                        ShortDescription = "Exhibition of student painting, live musical performances, theatre plays, and digital art designs.",
                        FullDescription = "<p>Experience the cultural side of Green Field Public School. This event features live visual arts exhibits, classical and contemporary music performances, and short plays performed by our theater society.</p>",
                        ImagePath = "/images/img_5.jpg",
                        GalleryImagesJson = "[\"/images/img_39.jpg\"]",
                        RegistrationLink = "",
                        IsPublished = true
                    }
                };

                context.Events.AddRange(events);
            }

            // 7. Seed News
            if (!context.News.Any())
            {
                var newsList = new List<News>
                {
                    new News
                    {
                        Title = "Green Field Public School Ranked #1 Green School",
                        Slug = "green-field-ranked-1-green-school",
                        PublishDate = DateTime.Now.AddDays(-5),
                        Summary = "Awarded the Green Campus Initiative accolade for our eco-friendly architecture, solar power systems, and recycling structures.",
                        Content = "<p>We are thrilled to announce that Green Field Public School has been named the leading green school in the district. This award highlights our investments in building a fully sustainable campus ecosystem, including rainwater harvesting, organic composting, and solar panel arrays feeding the smart grids.</p><p>We believe environmental responsibility is a core tenet of education. Our students take part in recycling, planting trees, and monitoring resource usage across classrooms.</p>",
                        ImagePath = "/images/img_11.jpg",
                        Author = "Principal's Office",
                        IsFeatured = true,
                        IsPublished = true
                    },
                    new News
                    {
                        Title = "Robotics Team Triumphs at Nationals",
                        Slug = "robotics-team-triumphs-at-nationals",
                        PublishDate = DateTime.Now.AddDays(-12),
                        Summary = "Our student robotics squad secured first place in the National coding and hardware arena with their autonomous maze solver.",
                        Content = "<p>Our senior coding squad designed, tested, and coded an autonomous vehicle that completed the national arena maze challenge in record time. Using machine vision and PID loops, the vehicle performed flawlessly. The team has been invited to represent the country at the international level in Tokyo.</p>",
                        ImagePath = "/images/img_12.jpg",
                        Author = "Robotics Coach",
                        IsFeatured = false,
                        IsPublished = true
                    }
                };

                context.News.AddRange(newsList);
            }

            // 8. Seed Gallery Albums and Items
            if (!context.GalleryAlbums.Any())
            {
                var albums = new List<GalleryAlbum>
                {
                    new GalleryAlbum
                    {
                        Name = "Campus Life & Infrastructure",
                        Description = "Sneak peek into our green campus, buildings, and smart labs.",
                        CoverImageUrl = "/images/img_37.jpg",
                        EventTag = "Campus",
                        IsActive = true
                    },
                    new GalleryAlbum
                    {
                        Name = "Sports Meet 2024",
                        Description = "Glimpses of athletic events and championships.",
                        CoverImageUrl = "/images/img_38.jpg",
                        EventTag = "Sports",
                        IsActive = true
                    },
                    new GalleryAlbum
                    {
                        Name = "Annual Arts & Culture Fest",
                        Description = "Captured moments of music, dance, drama and fine arts.",
                        CoverImageUrl = "/images/img_39.jpg",
                        EventTag = "Cultural",
                        IsActive = true
                    }
                };
                context.GalleryAlbums.AddRange(albums);
                await context.SaveChangesAsync();

                var albumList = await context.GalleryAlbums.ToListAsync();
                var campusAlbum = albumList[0];
                var sportsAlbum = albumList[1];
                var culturalAlbum = albumList[2];

                var gallery = new List<GalleryItem>
                {
                    new GalleryItem
                    {
                        Title = "Advanced Science Lab",
                        Category = "Academics",
                        FilePath = "/images/img_37.jpg",
                        IsVideo = false,
                        GalleryAlbumId = campusAlbum.Id
                    },
                    new GalleryItem
                    {
                        Title = "Class of 2024 Graduation",
                        Category = "Campus",
                        FilePath = "/images/img_40.jpg",
                        IsVideo = false,
                        GalleryAlbumId = campusAlbum.Id
                    },
                    new GalleryItem
                    {
                        Title = "Robotics & Coding Workshop",
                        Category = "Academics",
                        FilePath = "/images/img_41.jpg",
                        IsVideo = false,
                        GalleryAlbumId = campusAlbum.Id
                    },
                    new GalleryItem
                    {
                        Title = "Annual Athletics Meet",
                        Category = "Sports",
                        FilePath = "/images/img_38.jpg",
                        IsVideo = false,
                        GalleryAlbumId = sportsAlbum.Id
                    },
                    new GalleryItem
                    {
                        Title = "Spring Arts Festival",
                        Category = "Cultural",
                        FilePath = "/images/img_39.jpg",
                        IsVideo = false,
                        GalleryAlbumId = culturalAlbum.Id
                    }
                };

                context.GalleryItems.AddRange(gallery);
            }

            // 9. Seed Facilities
            if (!context.Facilities.Any())
            {
                var facilities = new List<Facility>
                {
                    new Facility
                    {
                        Name = "The Main Atrium",
                        Description = "Luminous, high-ceiling reception and collaboration zone representing our modern corporate academic visual design.",
                        Icon = "domain",
                        ImagePath = "/images/img_31.jpg",
                        Details = "Constructed with low-emission glass and custom acoustic paneling, the atrium serves as the campus hub where students collaborate, study, and hold exhibitions. Fully air-conditioned and Wi-Fi enabled.",
                        DisplayOrder = 1
                    },
                    new Facility
                    {
                        Name = "Science & Innovation Labs",
                        Description = "Sophisticated Physics, Chemistry, Biology, and STEM workspaces equipped with top-tier research gear.",
                        Icon = "biotech",
                        ImagePath = "/images/img_37.jpg",
                        Details = "Includes gas lines, safety hood vents, high-resolution microscopes, and IoT makerspace tables. Accommodates up to 30 students per lab session under expert teacher supervision.",
                        DisplayOrder = 2
                    },
                    new Facility
                    {
                        Name = "Modern Library",
                        Description = "A serene environment containing 20,000+ volumes, electronic reference terminals, and private group study units.",
                        Icon = "local_library",
                        ImagePath = "/images/img_34.jpg",
                        Details = "Equipped with digital book scanners, global database access terminals, comfortable ergonomic seating, and silent zones for deep reading and analysis.",
                        DisplayOrder = 3
                    },
                    new Facility
                    {
                        Name = "Olympic-Size Sports Complex",
                        Description = "Complete outdoor soccer field, running tracks, indoor basketball courts, and professional training locker rooms.",
                        Icon = "sports_basketball",
                        ImagePath = "/images/img_38.jpg",
                        Details = "Includes custom shock-absorbent synthetic courts, floodlights for night tournaments, electronic scoring boards, and dedicated spaces for athletic training.",
                        DisplayOrder = 4
                    }
                };

                context.Facilities.AddRange(facilities);
            }

            // 10. Seed Achievements
            if (!context.Achievements.Any())
            {
                var achievements = new List<Achievement>
                {
                    new Achievement
                    {
                        Title = "Best Educational Institution",
                        RecipientName = "Green Field Public School",
                        Year = "2024",
                        AchievementDate = DateTime.Now.AddMonths(-1),
                        AchievementType = "School",
                        Description = "Recognized at the National Education Awards for outstanding integration of tech and academic excellence.",
                        Category = "Accolades",
                        Icon = "workspace_premium",
                        ImagePath = "/images/img_11.jpg"
                    },
                    new Achievement
                    {
                        Title = "Varsity Soccer Championship Winners",
                        RecipientName = "Under-19 Football Team",
                        Year = "2023",
                        AchievementDate = DateTime.Now.AddMonths(-6),
                        AchievementType = "Student",
                        Description = "Our varsity sports squad claimed the state cup after going undefeated during the entire regular season.",
                        Category = "Athletics",
                        Icon = "sports_soccer",
                        ImagePath = "/images/img_38.jpg"
                    },
                    new Achievement
                    {
                        Title = "MIT Admission & Scholarship",
                        RecipientName = "Aisha Sharma",
                        Year = "2024",
                        AchievementDate = DateTime.Now.AddMonths(-3),
                        AchievementType = "Student",
                        Description = "Alumni Aisha Sharma secured a fully funded scholarship to MIT to pursue aerospace engineering after robotics laurels.",
                        Category = "Student",
                        Icon = "person",
                        ImagePath = "/images/img_32.jpg"
                    }
                };

                context.Achievements.AddRange(achievements);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string email, string fullName, string role, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
