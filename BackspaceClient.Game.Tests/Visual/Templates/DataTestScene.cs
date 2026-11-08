using System;
using BackspaceClient.Game.Online.API.Response;

namespace BackspaceClient.Game.Tests.Visual.Templates;

public abstract partial class DataTestScene : BackspaceClientTestScene
{
    protected readonly Random Random = new();

    protected string RandomString(int length)
    {
        const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz #:?.\"!@$%^&*()-_=+[]{}|;:',<>/?~`" +
            "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿ" +
            "ĄąĆćĘęŁłŃńŚśŹźŻżČčĎďĚěŇňŘřŠšŤťŮůŽžĞğİıŞşŒœŠšŸŽžǺǻǼǽǾǿ" +
            "ĂăÂâĐđÊêÔôƠơƯưẠạẢảẤấẦầẨẩẪẫẬậẮắẰằẲẳẴẵẶặẸẹẺẻẼẽẾếỀềỂểỄễỆệỈỉỊịỌọỎỏỐốỒồỔổỖỗỘộỚớỜờỞởỠỡỢợỤụỦủỨứỪừỬửỮữỰựỲỳỴỵỶỷỸỹ" +
            "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
            "ӘәӨөҮүҖҗҢңҺһІіЇїЄєҐґЎўЂђЃѓЉљЊњЋћЌќЏџЅѕҚқҒғҰұІіҺһҘҙҪҫҠҡ" +
            "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψωςΆΈΉΊΌΎΏάέήίόύώϊϋΐΰ" +
            //"აბგდევზთიკლმნოპჟრსტუფქღყშჩცძწჭხჯჰ" +
            //"ԱԲԳԴԵԶԷԸԹԺԻԼԽԾԿՀՁՂՃՄՅՆՇՈՉՊՋՌՍՎՏՐՑՒՓՔՕՖաբգդեզէըթժիլխծկհձղճմյնշոչպջռսվտրցւփքօֆ" +
            //"אבגדהוזחטיךכלםמןנסעףפץצקרשת" +
            //"ابتثجحخدذرزسشصضطظعغفقكلمنهويءآأؤإئابةتثجحخدذرزسشصضطظعغفقكلمنهوىيپچژکگی" +
            //"अआइईउऊऋएऐओऔकखगघङचछजझञटठडढणतथदधनपफबभमयरलवशषसह" +
            //"অআইঈউঊঋএঐওঔকখগঘঙচছজঝঞটঠডঢণতথদধনপফবভমযরলশষসহ" +
            //"กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮ" +
            //"ཀཁགངཅཆཇཉཏཐདནཔཕབམཙཚཛཝཞཟའཡརལཤསཧཨ" +
            //"ሀሁሂሃሄህሆለሉሊላሌልሎሐሑሒሓሔሕሖመሙሚማሜምሞሠሡሢሣሤሥሦረሩሪራሬርሮሰሱሲሳሴስሶሸሹሺሻሼሽሾ" +
            "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん" +
            "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンヴヵヶ" +
            "的一是不了人我在有他这为之大来以个中上们到说国和地也子时道出而要于就下得可你年生自会那后能对着事其里所去行过家十用发天如然作方成者多日都三小军二无同么经法当起与好看学进种将还分此心前面又定见只主没公从" +
            "龍龜鳳凰麒麟鬱籲爨戀攣癵虋齉齾龗龖厵滟" +
            "가나다라마바사아자차카타파하각난닫랄맘밥삿앙잦챀캌탙팝핳한글테스트문자열";
        //"ᎠᎡᎢᎣᎤᎥᎦᎧᎨᎩᎪᎫᎬᎭᎮᎯᎰᎱᎲᎳᎴᎵᎶᎷᎸᎹᎺᎻᎼᎽᎾᎿᏀᏁᏂᏃᏄᏅᏆ QUE" +
        //"ᐁᐂᐃᐄᐅᐆᐇᐈᐉᐊᐋᐌᐍᐎᐏᐐᐑᐒᐓᐔᐕᐖᐗᐘᐙᐚᐛᐜᐝᐞᐟᐠᐡᐢᐣᐤᐥᐦᐧᐨᐩᐪᐫᐬᐭᐮᐯᐰᐱᐲᐳᐳ埔" +
        //"ᚠᚡᚢᚣᚤᚥᚦᚧᚨᚩᚪᚫᚬᚭᚮᚯᚰᚱᚲᚳᚴᚵᚶᚷᚸᚹᚺᚻᚼᚽᚾᚿᛀᛁᛂᛃᛄᛅᛆᛇᛈᛉᛊᛋᛌᛍᛎᛏᛐᛑᛒᛓᛔᛕᛖᛗᛘᛙᛚᛛᛜᛝᛞᛟ" +
        //"€£¥¢₹₽₿₩₪₫₴₸₺₼₾±×÷≠≈≤≥∞∑∏√∫∆∇∂∀∃∈∉∋∅∩∪⊂⊃⊆⊇∧∨¬⊕⊗⊥⊤⊢⊣§¶©®™°µΩ";

        return string.Create(length, chars, (span, pool) =>
        {
            for (int i = 0; i < span.Length; i++)
                span[i] = pool[Random.Next(pool.Length)];
        });
    }

    protected User CreateUser()
    {
        return new User()
        {
            Id = RandomString(15),
            Username = RandomString(10),
            DisplayName = RandomString(5),
        };
    }

    protected DmChatMessageResponse CreateMessage(bool edited = false, bool large = true)
    {
        var user = CreateUser();
        var proc = Random.NextSingle() * 100;
        return new()
        {
            Id = RandomString(15),
            ChannelId = RandomString(15),
            UserId = user.Id,
            Content = large ? RandomString(25 * (int)proc) : RandomString(25),
            User = user,
            EditedAt = edited ? Random.NextInt64() : null
        };
    }
}
