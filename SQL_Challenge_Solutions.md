# SQL Challenge Solutions

## Part 3: SQL Challenge

---

## 1. Database Indexing Strategy

### What is an Index?

An **index** is a database structure that improves the speed of data retrieval operations on a table. Think of it like a book's index - instead of reading every page to find a topic, you check the index to jump directly to the relevant page.

### Types of Indexes

#### A. Clustered Index
- Determines the physical order of data in the table
- Only **ONE** clustered index per table
- Usually created automatically on the **Primary Key**
- Data is sorted and stored based on the clustered index key

**Example:**
```sql
CREATE CLUSTERED INDEX IX_Users_Id ON Users(Id);
```

#### B. Non-Clustered Index
- Creates a separate structure that points to the actual data
- **Multiple** non-clustered indexes allowed per table
- Used for frequently searched columns
- Does not affect the physical order of data

**Example:**
```sql
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
```

#### C. Composite Index
- Index on multiple columns
- Useful when queries filter by multiple columns together
- Column order matters (most selective column first)

**Example:**
```sql
CREATE INDEX IX_Students_Subject_Score ON Students(Subject, Score DESC);
```

#### D. Unique Index
- Ensures all values in the indexed column are unique
- Automatically created for PRIMARY KEY and UNIQUE constraints
- Prevents duplicate values

**Example:**
```sql
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
```

---

### When to Use Indexing

#### ✅ CREATE Index When:

1. **Columns in WHERE clause**
   ```sql
   SELECT * FROM Users WHERE Email = 'john@example.com';
   -- Create index on Email column
   ```

2. **Columns in JOIN operations**
   ```sql
   SELECT * FROM Orders o JOIN Customers c ON o.CustomerId = c.Id;
   -- Create index on CustomerId (Foreign Key)
   ```

3. **Columns in ORDER BY clause**
   ```sql
   SELECT * FROM Products ORDER BY Price DESC;
   -- Create index on Price column
   ```

4. **Columns in GROUP BY clause**
   ```sql
   SELECT Category, COUNT(*) FROM Products GROUP BY Category;
   -- Create index on Category column
   ```

5. **Large tables (1000+ rows)**
   - More data = more benefit from indexing

6. **High cardinality columns**
   - Columns with many unique values (Email, Phone, SSN)
   - Better index selectivity

#### ❌ AVOID Index When:

1. **Small tables (< 1000 rows)**
   - Full table scan is faster than index lookup

2. **Columns with frequent INSERT/UPDATE/DELETE**
   - Each data modification requires index update
   - Slows down write operations

3. **Low cardinality columns**
   - Few unique values (Gender: M/F, Status: Active/Inactive)
   - Index provides minimal benefit

4. **Columns rarely used in queries**
   - Wastes storage space
   - Slows down data modifications


---

## 2. Find Top Performers - SQL Query

### Problem Statement

Given a Students table:
```sql
CREATE TABLE Students (
    StudentID INTEGER,
    Name VARCHAR(100),
    Subject VARCHAR(100),
    Score INTEGER
);
```

**Task:** Find the top scorer in each subject.

---

### Sample Data

```sql
INSERT INTO Students (StudentID, Name, Subject, Score) VALUES
(1, 'Alice', 'Math', 95),
(2, 'Bob', 'Math', 88),
(3, 'Charlie', 'Math', 92),
(4, 'Alice', 'Science', 90),
(5, 'Bob', 'Science', 95),
(6, 'Charlie', 'Science', 87),
(7, 'Alice', 'English', 92),
(8, 'Bob', 'English', 89),
(9, 'Charlie', 'English', 95);
```

---

### Solution: Using Subquery 

```sql
SELECT s1.StudentID, s1.Name, s1.Subject, s1.Score
FROM Students s1
WHERE s1.Score = (
    SELECT MAX(s2.Score)
    FROM Students s2
    WHERE s2.Subject = s1.Subject
);
```

**Output:**
```
StudentID | Name    | Subject  | Score
----------|---------|----------|-------
1         | Alice   | Math     | 95
5         | Bob     | Science  | 95
9         | Charlie | English  | 95
```

**How it works:**
1. For each student, find the MAX score in their subject
2. Return students whose score equals the MAX score
3. Handles ties automatically (returns all students with max score)


## End of SQL Challenge Solutions
