// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

using System.Net;
using System.IO;
//using System.String;
using System.Text;
using System;
using Newtonsoft.Json;



namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        public dynamic JNo { get; set; }
        public dynamic JDes { get; set; }
        public dynamic JInv { get; set; }
        public dynamic JUom { get; set; }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {


            string url = @"http://nddemo.southeastasia.cloudapp.azure.com:7048/BC140/ODataV4/Company('ND')/ItemList?$filter=No%20eq%20'";
            //string url = @"http://clbmobile.southeastasia.cloudapp.azure.com:7048/NAV/ODataV4/Company('CRONUS International Ltd.')/Item_list?$filter=No eq %27";
            string itemNo = turnContext.Activity.Text;
            string url2 = url + itemNo+"'";
            WebRequest request = WebRequest.Create(url2);
            request.Method = "GET";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("Sa:Newdawn999"));
            WebResponse MyWebResponse = request.GetResponse();

            Stream data = MyWebResponse.GetResponseStream();

            StreamReader reader = new StreamReader(data);

            // json-formatted string from maps api
            string responseFromServer = reader.ReadToEnd();

            var JData = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
            JNo = JData.value[0].No;
            JDes = JData.value[0].Description;
            JInv = JData.value[0].Inventory;
            JUom = JData.value[0].Base_Unit_of_Measure;
            string AnswerBack = "Item No =" + JNo +" "+ JDes +" "+ "Inventory = " + JInv +" " +JUom;
            MyWebResponse.Close();

            //await turnContext.SendActivityAsync(MessageFactory.Text($"==>:{url2}"), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"{AnswerBack}"), cancellationToken);
            //await turnContext.SendActivityAsync(MessageFactory.Text($"{responseFromServer}"), cancellationToken);
        }
        

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello World!"), cancellationToken);
                }
            }
        }
    }
}
