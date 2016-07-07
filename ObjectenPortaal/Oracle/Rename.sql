declare 
  cursor c_coda_columns is 
    select
      t.owner
      , t.table_name
      , c.column_name
    from SDE.TABLE_REGISTRY t
    join SDE.COLUMN_REGISTRY c on t.table_name = c.table_name and t.owner = c.owner
    where 1=1
      and ( lower(c.COLUMN_NAME) = 'coda_1e' 
           or lower(c.COLUMN_NAME) = 'coda_3e');
  r_coda_columns c_coda_columns%rowtype;
  new_column_name varchar2(200);
begin
  open c_coda_columns;
  loop
    fetch c_coda_columns into r_coda_columns;
    exit when c_coda_columns%notfound;
    
    if (r_coda_columns.column_name = 'CODA_1E') then new_column_name := 'CODA_2E';
    elsif (r_coda_columns.column_name = 'CODA_3E') then new_column_name := 'CODA_4E';
    else new_column_name := 'DEZEBESTAATZEKERNIET';
    end if;
    
    -- execute immediate
    dbms_output.put_line('alter table ' || r_coda_columns.owner || '.' || r_coda_columns.table_name || ' rename column ' || r_coda_columns.column_name || ' to ' || new_column_name);
    
    /*
    update sde.column_registry 
    set column_name = new_column_name
    where 1=1
      and owner = r_coda_columns.owner
      and table_name = r_coda_columns.table_name
      and column_name = r_coda_columns.column_name;
    */
  end loop;
  close c_coda_columns;
end;
/