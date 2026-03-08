using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Shared;

public sealed record LoginRequest(string Username, string Password);