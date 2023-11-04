using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xan.TimeTracker.Models;

public record CommentSummary(
    string Comment,
    TimeSpan TotalDuration,
    IReadOnlyCollection<Duration> Details
);
