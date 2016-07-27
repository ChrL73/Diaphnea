#include "mongo/client/dbclient.h"
#include "mongo/bson/bson.h"

#include <iostream>

void test2(mongo::DBClientConnection& c, int index)
{
    char projectionStr[64];
    sprintf(projectionStr, "{ questions: { $slice: [%d, 1] } }", index);
    mongo::BSONObj projection = mongo::fromjson(projectionStr);
    auto cursor = c.query("diaphnea.question_lists", MONGO_QUERY( "_id" << mongo::OID("579201b9db404c2f801240f7")), 1, 0, &projection);

    while (cursor->more())
    {
        mongo::BSONObj question = cursor->next();
        std::cout << question.toString() << std::endl << std::endl;
        std::cout << question.getStringField("questionnaire") << std::endl << std::endl;
        std::cout << question.getIntField("count") << std::endl << std::endl;

        std::vector<mongo::BSONElement> v = question.getField("questions").Array();
        std::cout << v[0].Obj().getField("question").toString() << std::endl << std::endl;
    }
}

void test1(mongo::DBClientConnection& c)
{
    mongo::BSONObjBuilder b;
    b.append("name", "Joe");
    b.append("age", 33);
    mongo::BSONObj p = b.obj();

    c.insert("tutorial.persons", p);

    std::cout << "count:" << c.count("tutorial.persons") << std::endl;

    auto cursor = c.query("tutorial.persons", mongo::BSONObj());
    while (cursor->more()) std::cout << cursor->next().toString() << std::endl;
}

void run(void)
{
    mongo::DBClientConnection c;
    c.connect("localhost");

    //test1(c);
    test2(c, 4);
}

int main(void)
{
    mongo::client::initialize();
    try
    {
        run();
    }
    catch(const mongo::DBException& e)
    {
        std::cout << "caught " << e.what() << std::endl;
    }
    return 0;
}
