﻿using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
