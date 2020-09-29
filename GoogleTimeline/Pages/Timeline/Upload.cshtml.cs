using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Model.Timeline.External;
using GoogleTimelineUI.Services;
using DataAccess;

namespace GoogleTimeline.Pages.Timeline
{
    [Authorize]
    public class UploadModel : PageModel
    {
        private readonly TimelineRepository _timelineRepository;
        private readonly UserService _userService;

        public bool UploadCompleted { get; set; }

        public UploadModel(TimelineRepository timelineRepository, UserService userService)
        {
            _timelineRepository = timelineRepository;
            _userService = userService;
        }

        public async Task OnPost()
        {
            if (Request.Form.Files.Count > 0)
            {
                var serialize = new JsonSerializer();
                var timelines = Request.Form.Files.Select(file =>
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            using (var jtr = new JsonTextReader(sr))
                            {
                                return serialize.Deserialize<SemanticTimeline>(jtr);
                            }
                        }
                    }
                });

                _timelineRepository.AddTimelineData(await _userService.CurrentUser(), timelines);
                UploadCompleted = true;
            }
        }
    }
}
