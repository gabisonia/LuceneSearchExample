using System.Globalization;
using Lucene.Net.Documents;
using LuceneSearchExample.Search.Models;

namespace LuceneSearchExample.Search.Indexing;

internal static class LuceneDocumentFactory
{
    public static Document Create(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return
        [
            new StringField(FieldNames.UserId, user.UserId.ToString(CultureInfo.InvariantCulture), Field.Store.YES),
            new TextField(FieldNames.FirstName, user.FirstName, Field.Store.YES),
            new TextField(FieldNames.LastName, user.LastName, Field.Store.YES),
            new StringField(FieldNames.Age, user.Age.ToString(CultureInfo.InvariantCulture), Field.Store.YES)
        ];
    }

    public static User ToUser(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var userId = Convert.ToInt32(document.Get(FieldNames.UserId), CultureInfo.InvariantCulture);
        var firstName = document.Get(FieldNames.FirstName) ?? string.Empty;
        var lastName = document.Get(FieldNames.LastName) ?? string.Empty;
        var age = Convert.ToInt32(document.Get(FieldNames.Age), CultureInfo.InvariantCulture);
        return new User(userId, firstName, lastName, age);
    }
}
