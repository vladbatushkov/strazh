MATCH (n)
WHERE n:Class OR n:Interface
UNWIND split(n.modifiers, ",") as x
WITH x, n
WHERE x in ["public", "internal"]
RETURN labels(n)[0] as name, x as modifier, count(n) as count

MATCH (n)
WHERE (n:Class OR n:Interface)
AND n.modifiers =~ ".*public.*"
RETURN labels(n)[0] as name, n.modifiers as modifier, count(n) as count

MATCH (n)-[:DECLARED_AT]->(:File)-[:INCLUDED_IN*]->(:Folder)<-[:INCLUDED_IN]-(p:Project)
WHERE p.name = "Agoda.Cronos.Bootstrap"
AND (n:Class OR n:Interface)
AND n.modifiers =~ ".*public.*"
RETURN labels(n)[0] as name, count(n) as count

MATCH (m:Method)<-[:HAVE]-(c:Class)-[:DECLARED_AT]->(f:File)
WHERE f.fullName =~ "Agoda.Cronos.Bootstrap.*"
AND NOT f.fullName =~ "Agoda.Cronos.BootstrapUnitTests.*"
AND m.modifiers =~ ".*public.*"
AND c.modifiers =~ ".*public.*"
RETURN labels(m)[0] as name,
//c.modifiers as class,
//m.modifiers as method,
count(m) as count