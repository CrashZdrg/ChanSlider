using ChanSlider.Api;
using ChanSlider.Models;
using ChanSlider.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ChanSlider.Presenters
{
    class MainPresenter
    {
        private const int IMGBUFFER = 5;

        readonly IMainWindow _window;
        readonly ConfigMdl _cfg;

        readonly System.Timers.Timer _timer;

        readonly IReadOnlyList<BaseApi> _apis;

        List<ApiItemMdl> apiItemBuffer;
        int currentApiItemsIndex;
        int bufferApiItemsIndex;

        Utils.LogFile log;

        public MainPresenter(IMainWindow window)
        {
            _window = window;

            _cfg = Utils.JsonFile.Get<ConfigMdl>("Config.json");

            _timer = new System.Timers.Timer(_cfg.IntervalS <= 0 ? 5000 : (_cfg.IntervalS * 1000))
            {
                AutoReset = false,
            };
            _timer.Elapsed += Timer_Elapsed;

            _apis = new List<BaseApi>()
            {
#if DEBUG
                new TestApi(),
#endif
                new KonachanApi(),
                new LoliApi(),
                new DanbooruApi(),
            };

            window.Apis = _apis.Select(x => x.GetType().Name.Replace("Api", string.Empty));

            window.Load += Window_Load;
            window.Closing += Window_Closing;

            window.Start += Window_Start;
            window.Stop += Window_Stop;

            window.NextImage += Window_NextImage;
            window.PrevImage += Window_PrevImage;

            window.GoPost += Window_GoPost;
        }

        private void Window_GoPost()
        {
            if (!_window.Running)
                return;

            if (currentApiItemsIndex <= 0)
                return;

            string postUrl = apiItemBuffer[currentApiItemsIndex - 1].PostUrl;

            if (string.IsNullOrWhiteSpace(postUrl))
                return;

            System.Diagnostics.Process.Start(postUrl);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _window.Dispatcher.BeginInvoke(_window.TimerTick);
        }

        private void Window_NextImage()
        {
            if (!_window.Running)
                return;

            if (_window.RenderView.IsSwitching)
                return;

            if (currentApiItemsIndex < apiItemBuffer.Count)
            {
                _timer.Stop();
                _window.RenderView.SwitchImages();
            }
        }

        private void Window_PrevImage()
        {
            if (!_window.Running)
                return;

            if (_window.RenderView.IsSwitching)
                return;

            if (currentApiItemsIndex > 2)
            {
                _timer.Stop();

                currentApiItemsIndex -= 3;
                _window.RenderView.SetNextImage(apiItemBuffer[currentApiItemsIndex].Source);
                currentApiItemsIndex++;

                _window.RenderView.SwitchImages();
            }
        }

        private async void Window_Start()
        {
            _window.RenderView.IsLoading = true;

            SaveConfig();

            _window.RenderView.AnimationDuration =
                _cfg.AnimationDurationMs > 0 ?
                new Duration(TimeSpan.FromMilliseconds(_cfg.AnimationDurationMs)) :
                Duration.Automatic;

            var _api = _apis[_cfg.Api];

            apiItemBuffer = await _api.GetItemsAsync(_cfg.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), _cfg.HighRes);

            log = new Utils.LogFile("log.log");

            bufferApiItemsIndex = Math.Min(IMGBUFFER, apiItemBuffer.Count);

            for (int i = 0; i < bufferApiItemsIndex; i++)
            {
                apiItemBuffer[i].Source = new BitmapImage(apiItemBuffer[i].Url);
                log.WriteLine(apiItemBuffer[i].PostUrl);
            }

            _window.RenderView.NextImage += RenderView_NextImage;
            _window.RenderView.IsLoading = false;

            if (currentApiItemsIndex < apiItemBuffer.Count)
            {
                _window.RenderView.SetNextImage(apiItemBuffer[currentApiItemsIndex].Source);
                currentApiItemsIndex++;
            }

            _window.RenderView.SwitchImages();
        }

        private void RenderView_NextImage()
        {
            if (!_window.Running)
                return;

            if (currentApiItemsIndex < apiItemBuffer.Count)
            {
                _window.RenderView.SetNextImage(apiItemBuffer[currentApiItemsIndex].Source);
                currentApiItemsIndex++;
            }

            if (bufferApiItemsIndex < apiItemBuffer.Count && bufferApiItemsIndex - (currentApiItemsIndex - 2) <= IMGBUFFER)
            {
                apiItemBuffer[bufferApiItemsIndex].Source = new BitmapImage(apiItemBuffer[bufferApiItemsIndex].Url);
                log.WriteLine(apiItemBuffer[bufferApiItemsIndex].PostUrl);
                bufferApiItemsIndex++;
            }

            if (currentApiItemsIndex < apiItemBuffer.Count)
            {
                if (_cfg.IntervalS > 0)
                    _timer.Start();
            }
        }

        private void Window_Stop()
        {
            _timer.Stop();

            apiItemBuffer = null;
            bufferApiItemsIndex = 0;
            currentApiItemsIndex = 0;

            log.Dispose();
            log = null;
        }

        private void SaveConfig()
        {
            _cfg.Tags = (_window.Tags ??= string.Empty);
            _cfg.Fullscreen = _window.Fullscreen;
            _cfg.HighRes = _window.HighRes;

            _cfg.AnimationDurationMs =
                _window.AnimationDuration < 0 ?
                (_window.AnimationDuration = 0) :
                _window.AnimationDuration;

            _cfg.IntervalS =
                _window.Interval < 0 ?
                (_window.Interval = 0) :
                    (_window.Interval * 1000) > _window.AnimationDuration ?
                    (_window.Interval = _window.AnimationDuration / 1000) :
                _window.Interval;

            _cfg.Api = _window.SelectedApi;

            _cfg.SaveJson();
        }

        private void Window_Closing()
        {
            SaveConfig();
        }

        private void Window_Load()
        {
            _window.Tags = _cfg.Tags;
            _window.Fullscreen = _cfg.Fullscreen;
            _window.HighRes = _cfg.HighRes;
            _window.AnimationDuration = _cfg.AnimationDurationMs;
            _window.Interval = _cfg.IntervalS;

            _window.SelectedApi =
                _cfg.Api < 0 || _cfg.Api >= _apis.Count ?
                (_cfg.Api = 0) :
                _cfg.Api;
        }
    }
}
