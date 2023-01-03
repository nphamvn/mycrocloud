using MNB.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;
using MNB.Web.Models.Shared;

namespace MNB.Web.Services
{
    public interface IFlashMessageService
    {
        FlashMessageWithUrl JsonModel();

        void Set(FlashMessage flashMessage, string pathName = null);

        void Set(MessageRouteSet messageRouteSet);

        // todo 1 実装の移行のための暫定メソッド。実装移行が終われば廃止
        void Set(string message, string action = Constants.Action.Index);
    }

    public class FlashMessageService : IFlashMessageService
    {
        private static string TempDataKey { get; set; } = "FlashMessage";

        private ITempDataDictionary _tempData;

        private ITempDataDictionary TempData
        {
            init => _tempData = value;
            get => _tempData ??= _tempDataFactory.GetTempData(_httpContextAccessor.HttpContext);
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public FlashMessageService(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataFactory = tempDataFactory;
        }

        private bool IsMatchedCurrentUrl(FlashMessageWithUrl model)
        {
            var pathName = _httpContextAccessor.HttpContext.Request.Path.Value;

            // todo 2 pathNameが空文字でも出力する場合があるか？
            if (string.IsNullOrEmpty(pathName))
            {
                return false;
            }

            return model.PathName == pathName;
        }

        public FlashMessageWithUrl JsonModel()
        {
            var serializedString = GetTempData();

            var model = GetDeserializedModel(serializedString);

            return ShouldRender(model) ? model : null;
        }

        private FlashMessageWithUrl GetDeserializedModel(string serializedString)
        {
            return string.IsNullOrEmpty(serializedString) ? null : JsonSerializer.Deserialize<FlashMessageWithUrl>(serializedString);
        }

        /// <summary>
        /// 取り出したら、TempDataからは削除されるので注意
        /// </summary>
        /// <returns></returns>
        private string GetTempData()
        {
            var data = TempData[TempDataKey];

            TempData.Remove(TempDataKey);

            return (string)data;
        }

        public void Set(FlashMessage flashMessage, string pathName)
        {
            // pathNameの空白は許可しない
            pathName = string.IsNullOrEmpty(pathName) ? _httpContextAccessor.HttpContext.Request.Path.Value : pathName;

            var model = new FlashMessageWithUrl()
            {
                Message = flashMessage.Message,
                Option = flashMessage.Option ?? new FlashMessageOption(),
                PathName = pathName,
            };

            SetTempData(model);
        }

        public void Set(string message, string action = Constants.Action.Index)
        {
            // メッセージを表示するのは一覧画面が一般的なので。indexを既定値とする
            var routeSet = _httpContextAccessor.HttpContext.GetRouteSet(action);

            Set(new FlashMessage() { Message = message }, _httpContextAccessor.HttpContext.GetPathName(routeSet));
        }

        public void Set(MessageRouteSet messageRouteSet)
        {
            var routeSet = _httpContextAccessor.HttpContext.GetPathName(messageRouteSet.RouteSet);

            Set(messageRouteSet.Message, routeSet);
        }

        private void SetTempData(FlashMessageWithUrl model)
        {
            TempData[TempDataKey] = JsonSerializer.Serialize(model);
        }

        private bool ShouldRender(FlashMessageWithUrl model)
        {
            return model != null && !string.IsNullOrEmpty(model.Message) && IsMatchedCurrentUrl(model);
        }
    }
}